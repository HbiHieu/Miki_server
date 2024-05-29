using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ntier.BLL.Interfaces;
using Ntier.DAL.Entities;
using Ntier.DAL.Interfaces;
using Ntier.DTO.DTO.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MimeKit;

namespace Ntier.BLL.Services
{
    public class UserService :IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserService( IUserRepository userRepository , IMapper mapper , IConfiguration configuration ) {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ICollection<UserRegisterDTO>> GetUsersAsync( int pageSize , int pageIndex )
        {
            var users = await _userRepository.GetUsersAsync( pageSize , pageIndex );
            if ( users == null )
            {
                throw new Exception();
            }
            return _mapper.Map<ICollection<UserRegisterDTO>>( users );
        }

        public async Task RegisterUserAsync(UserRegisterDTO userDTO)
        {
            var user = await _userRepository.AddUserAsync( userDTO );
            if ( user == null )
            {
                throw new ArgumentException("Email already exits");
            }
        }

        public async Task<UserDTO?> LoginUserAsync(UserLoginDTO userDTO)
        {
            var user = await _userRepository.CheckUserAsync(userDTO);
            if ( user != null )
            {
                var jwtToken = await GenerateAccessToken(user);
                var refreshToken = await _userRepository.GetRefreshTokenAsync(user.Id);
                string refreshTk;
                if ( refreshToken != null )
                {
                    refreshTk = refreshToken.RefreshTk;
                }
                else
                {
                    refreshTk = await GenerateRefreshToken(user);
                }

                UserDTO userDto = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Access_token = jwtToken,
                    Expire_At = DateTime.UtcNow.AddMinutes(10),
                    Name = user.Name,
                    Refresh_Token = refreshTk,
                    Role = user.Role ,
                    PhoneNumber = user.PhoneNumber
                };
                return userDto;
            }
            else
            {
                throw new ArgumentException("Icorrect Email or password");
            }
        }

        public async Task<string> GenerateAccessToken( User user )
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetConnectionString("SecretKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role ),
            }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        public async Task<string> GenerateRefreshToken( User user )
        {
                var random = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(random);
                    string refreshToken = Convert.ToBase64String(random);
                RefreshToken refreshTk = new RefreshToken
                {
                    RefreshTk = refreshToken,
                    Userid = user.Id,
                    ExpireAt = DateTime.UtcNow.AddHours(10),
                };
                await _userRepository.AddRefreshTokenAsync(refreshTk);
                return refreshToken;
                }    
        }

        public async Task<string> GetNewAccessTokenAsync(string userId)
        {
            var refreshTk = await _userRepository.GetRefreshTokenAsync(userId);
            if (refreshTk != null)
            {
                if (refreshTk.ExpireAt < DateTime.UtcNow)
                {

                    await _userRepository.RemoveRefreshTokenAsync(userId);
                    throw new ArgumentException("Refresh token was expired");
                }
                else
                {
                    var user = await _userRepository.GetUserByIdAsync(userId);
                    var jwt = await GenerateAccessToken(user);
                    return jwt;
                }
            }
            else
            {
                throw new ArgumentException("UserId is invalid");
            }
        }
        public async Task SendEmailAsync(string recipientEmail, string subject, string messageBody)
        {
            var EmailMessage = new MimeMessage();
            EmailMessage.From.Add(new MailboxAddress("MiKi Shop", "noreply91203@gmail.com"));
            EmailMessage.To.Add(new MailboxAddress("", recipientEmail));
            EmailMessage.Subject = subject;
            EmailMessage.Body = new TextPart("plain")
            {
                Text = messageBody
            };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("noreply91203@gmail.com", "jyoxteofyiygapti");
                await client.SendAsync(EmailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
