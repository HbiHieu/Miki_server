using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ntier.BLL.Interfaces;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;
using Ntier.DTO.DTO.User;
using System.Collections.Concurrent;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private static readonly ConcurrentDictionary<string, string> _otpStorage = new ConcurrentDictionary<string, string>();
        private readonly ShopContext _shopContext;
        public UsersController( IUserService userService , ShopContext shopContext) {
            _userService = userService;
            _shopContext = shopContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser(int pageSize, int pageIndex)
        {
            try
            {
                var users = await _userService.GetUsersAsync(pageSize, pageIndex);
                return Ok(new
                {
                    data = users,
                    pagination = new
                    {
                        _totalRows = users.Count,
                        _page = pageIndex,
                        _limit = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser( UserRegisterDTO userDTO )
        {
            try
            {
                await _userService.RegisterUserAsync( userDTO );
                var cart = new Cart { CreateAt = DateTime.Now.ToString(), UserId = userDTO.Id };
                await _shopContext.Carts.AddAsync(cart);
                await _shopContext.SaveChangesAsync();
                return Ok(new { message = "Register successfully"  }) ;
            }
            catch ( Exception ex ) {
                return BadRequest( new { message = ex.Message } );
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser( UserLoginDTO userLoginDTO )
        {
            try
            {
                var user = await _userService.LoginUserAsync(userLoginDTO);
                var cart =  await _shopContext.Carts.FirstOrDefaultAsync(item => item.UserId == user.Id);
                if ( cart != null )
                {
                user.cartId = cart.Id;
                }
                return Ok( new
                {
                    data = user ,
                    message = "Login successfully"
                } );
            }
            catch( Exception ex )
            {
                return BadRequest( new { message = ex.Message } );
            }
        }

        [HttpPost("sendOtp")]
        public async Task<IActionResult> SendOTP([FromBody] UserRegisterDTO user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Cần phải nhập email");
            }
            string otp = generateOTP();
            _otpStorage[user.Email] = otp;
            string subject = "Your OTP for account verification";
            string message = "Your OTP is " + otp + ".";
            await _userService.SendEmailAsync(user.Email, subject, message);
            return Ok(new { message = "OTP sent successfully" });
        }

        private String generateOTP()
        {
            Random rd = new Random();
            return rd.Next(100000, 999999).ToString();
        }

        [HttpPost("verifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] UserRegisterDTO model, string Otp)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(Otp))
            {
                return BadRequest("Email and OTP are required.");
            }

            // Verify OTP
            if (_otpStorage.TryGetValue(model.Email, out var storedOtp) && storedOtp == Otp)
            {
                // OTP is valid, remove it from storage
                _otpStorage.TryRemove(model.Email, out _);
                await _userService.RegisterUserAsync(model);
                var cart = new Cart { CreateAt = DateTime.Now.ToString(), UserId = model.Id };
                await _shopContext.Carts.AddAsync(cart);
                await _shopContext.SaveChangesAsync();
                return Ok(new { Message = "Đăng ký thành công" });
            }

            return BadRequest("Invalid OTP.");
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> GetNewAccessToken( string userId )
        {
            try
            {
                string jwt = await _userService.GetNewAccessTokenAsync(userId);
                return Ok( new { 
                    jwt = jwt ,
                    expire_at = DateTime.UtcNow.AddMinutes(10),
                } );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser ( string userId )
        {
            try
            {
                var cart = await _shopContext.Carts.FirstOrDefaultAsync (item => item.UserId.Equals(userId));
                if ( cart != null )
                {
                    _shopContext.Carts.Remove( cart );                    
                }
                var refreshToken = await _shopContext.RefreshTokens.FirstOrDefaultAsync(item => item.Userid.Equals(userId));
                if ( refreshToken != null )
                {
                    _shopContext.RefreshTokens.Remove( refreshToken );
                }
                var user = await _shopContext.Users.FirstOrDefaultAsync(item => item.Id.Equals(userId));
                if ( user != null)
                {
                    _shopContext.Users.Remove( user );
                }
                await _shopContext.SaveChangesAsync();
                return Ok(new
                {
                    message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
