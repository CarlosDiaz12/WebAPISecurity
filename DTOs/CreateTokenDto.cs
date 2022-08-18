using Microsoft.IdentityModel.Tokens;
using System;

namespace WebAPISecurity.DTOs
{
    public class CreateTokenDto
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string RefreshToken { get; set; }
    }
}
