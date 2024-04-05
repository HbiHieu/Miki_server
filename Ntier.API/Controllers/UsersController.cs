using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ntier.BLL.Interfaces;
using Ntier.DTO.DTO.User;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
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
    }
}
