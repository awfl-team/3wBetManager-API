using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;

namespace _3wBetManager_API.Manager
{
    public class TokenManager
    {
        private const string Secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";

        public static string GenerateToken(string email, string role, string pseudo)
        {
            var key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Name, pseudo), 
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(securityKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                    parameters, out securityToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IDictionary<string,string> ValidateToken(string token)
        {
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }

            Claim emailClaim = identity.FindFirst(ClaimTypes.Email);
            Claim roleClaim = identity.FindFirst(ClaimTypes.Role);
            Claim pseudoClaim = identity.FindFirst(ClaimTypes.Name);
 
            IDictionary<string, string> tokenDictionary = new Dictionary<string, string>();
            tokenDictionary["pseudo"] = pseudoClaim.Value;
            tokenDictionary["email"] = emailClaim.Value;
            tokenDictionary["role"] = roleClaim.Value;

            return tokenDictionary;
        }

        public static string GetTokenFromRequest(HttpRequestMessage request)
        {
            if (!request.Headers.TryGetValues("Authorization", out var authHeaders) || authHeaders.Count() > 1)
            {
                return null;
            }
            var bearerToken = authHeaders.ElementAt(0);
            var token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return token;
        }


    }
}