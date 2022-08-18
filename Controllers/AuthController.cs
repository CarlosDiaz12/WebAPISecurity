using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPISecurity.DTOs;
using WebAPISecurity.Models;

namespace WebAPISecurity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly List<UserModel> _users = new List<UserModel>
        {
            new UserModel{ Id = 1, UserName = "karlus", Password = "123456", IsSuperUser = true},
            new UserModel{Id = 1, UserName = "karlus2", Password = "123456", IsSuperUser = false}
        };
        private readonly IConfiguration _config;
        public AuthController(IConfiguration configurationRoot)
        {
            _config = configurationRoot;
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] CreateRefreshTokenDto dto)
        {
            string savedRefreshToken = Request.Cookies["refreshToken"] ?? "QAAAAA=="; //temporal

            if (savedRefreshToken != dto.RefreshToken)
            {
                return Unauthorized();
            }
            var token = CreateToken();

            return Ok(token);
        }

        [HttpPost("Token")]
        public IActionResult CreateToken([FromBody] CredentialsModel credentials)
        {
            if (credentials == null) return BadRequest("Credentials cannot be null.");

            var user = _users.FirstOrDefault(x => x.UserName.Equals(credentials.UserName) && x.Password.Equals(credentials.Password));
            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sid, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("SuperUser", user.IsSuperUser.ToString())
                };

                var token = CreateToken(claims);
 
                return Ok(token);
            }

            return BadRequest("Failed to generate Token");
        }

        private CreateTokenDto CreateToken(Claim[] claims = null)
        {
            if(claims == null)
            {
                var handler = new JwtSecurityTokenHandler();
                var cleanToken = Request.Headers["Authorization"].ToString().Replace("Bearer", "").Trim();
                JwtSecurityToken tokenData = handler.ReadJwtToken(cleanToken);
                claims = tokenData.Claims.ToArray();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Auth:Key")));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(signingCredentials);

            // generate payload
            var payload = new JwtPayload(
                _config.GetValue<string>("Auth:Issuer"),
                _config.GetValue<string>("Auth:Audience"),
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(1)
                );

            // generate token
            var token = new JwtSecurityToken(header, payload);
            var refreshToken = CreateRefreshToken();
            SetRefreshToken(refreshToken);

            return new CreateTokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = token.ValidTo,
                RefreshToken = refreshToken
            };
        } 
        private void SetRefreshToken(string token)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(60),
                SameSite = SameSiteMode.Strict,
                Domain = "localhost",
                IsEssential = true,
                Secure = false
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string CreateRefreshToken()
        {
            var random = BitConverter.GetBytes(64);
            return Convert.ToBase64String(random);
        }

    }
}
