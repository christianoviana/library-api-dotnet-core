using Library.STS.Domain.DTO;
using Library.STS.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library.STS.Service
{
    public static class TokenService
    {
        private static SymmetricSecurityKey key;
        private static SigningCredentials signingCredentials;
    
        static TokenService()
        {
            // Store in a secret or in a config file
            string secret = "b9287d492b708eredaf7d8863b48f197redaf7d8jj";
            key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        }

        public static TokenInfoDto GenerateToken(User user)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                IssuedAt = DateTime.UtcNow,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = signingCredentials                
            };

            var securityToken = handler.CreateToken(securityTokenDescriptor);
            string token = handler.WriteToken(securityToken);

            return new TokenInfoDto()
            {
                Token = token, Created = securityTokenDescriptor?.IssuedAt.Value.ToLocalTime(),
                Expires = securityTokenDescriptor?.Expires.Value.ToLocalTime(), Message = "OK"
            };
        }

        public static TokenValidateDto ValidateToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromSeconds(1)
            };

            bool isValidToken = false;

            try
            {
                SecurityToken validatedToken;
                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);

                if (DateTime.Now <= validatedToken.ValidTo.ToLocalTime())
                {
                    isValidToken = true;
                }               
            }
            catch (SecurityTokenValidationException ex)
            {
                // Log the error to monitoring the api
            }
            catch(Exception ex)
            {
                // Log the error to monitoring the api
            }

            TokenValidateDto tokenValidate = new TokenValidateDto() { IsValidToken = isValidToken, Token = token, Message = isValidToken ? "OK" : "NOK" };

            return tokenValidate;
        }
    }
}