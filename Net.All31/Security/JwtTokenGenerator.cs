using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Net.Security
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        readonly SymmetricSecurityKey SecurityKey;
        readonly string Issuer;
        public JwtTokenGenerator(string secret,string issuer)
        {
            var bytes = Encoding.UTF8.GetBytes(secret);
            this.SecurityKey = new SymmetricSecurityKey(bytes);
            this.Issuer = issuer;
        }
        public string CreateToken(string userName, DateTime? expires=null, params KeyValuePair<string, string>[] claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var requestInfo = new ClaimsIdentity();
            requestInfo.AddClaim(new Claim(ClaimTypes.Name, userName));
            requestInfo.AddClaim(new Claim(ClaimTypes.NameIdentifier, userName));
            if (claims != null)
                requestInfo.AddClaims(claims.Select(p => new Claim(p.Key, p.Value)));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = requestInfo,
                Expires = expires ?? DateTime.Now.AddDays(1),
                Issuer = this.Issuer,
                SigningCredentials = new SigningCredentials(this.SecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return "Bearer " + tokenHandler.WriteToken(token);
        }
    }
}
