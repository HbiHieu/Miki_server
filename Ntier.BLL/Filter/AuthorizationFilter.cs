using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ntier.BLL.Filter
{
    public class AuthorizationFilter : IAuthorizationFilter
    {

        private readonly string _role;
        private readonly IConfiguration _configuration;
        public AuthorizationFilter(string role , IConfiguration configuration)
        {
            _role = role;
            _configuration = configuration;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request
                .Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetConnectionString("SecretKey"));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    //kiểm tra xem key có hợp lệ không 
                    ValidateIssuerSigningKey = true,
                    //key được dùng để validate
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //Không có sự chênh lệch time giữa 2 bên
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                if (jwtToken.Claims.First().Value != _role)
                {
                    context.Result = new UnauthorizedResult();
                }
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    // Token đã hết hạn
                    // Xử lý lỗi hoặc đăng nhập lại để tạo mới token
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
