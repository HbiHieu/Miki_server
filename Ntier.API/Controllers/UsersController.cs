using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ntier.BLL.Interfaces;
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
        public UsersController( IUserService userService ) {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser() {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok( users ); 
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser( UserRegisterDTO userDTO )
        {
            try
            {
               await _userService.RegisterUserAsync( userDTO );
               return Ok(new { message = "Register successfully" });
            }
            catch ( Exception ex ) {
                return BadRequest( new { message = ex.Message } );
            }

        }
        [HttpPost("sendOtp")]
        public async Task<IActionResult> SendOTP([FromBody] UserDTO user)
        {
            if(string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Cần phải nhập email");
            }
            string otp = generateOTP();
            _otpStorage[user.Email] = otp;
            string subject = "Your OTP for account verification";
            string message = "Your OTP is " + otp + ".";
            await _userService.SendEmailAsync(user.Email, subject, message);
            return Ok(new { message = "OTP sent successfully"});
        }
        [HttpPost("verifyOTP")]
        public IActionResult VerifyOTP([FromBody] UserDTO model, string Otp)
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
                return Ok(new { Message = "OTP verified successfully" });
            }

            return BadRequest("Invalid OTP.");
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser( UserLoginDTO userLoginDTO )
        {
            try
            {
                var user = await _userService.LoginUserAsync(userLoginDTO);
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
        private String generateOTP()
        {
            Random rd = new Random();
            return rd.Next(100000, 999999).ToString();
        }
    }
}
