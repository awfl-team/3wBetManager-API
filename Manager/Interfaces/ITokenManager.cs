using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;

namespace Manager
{
    public interface ITokenManager
    {
        string GenerateEmailToken(string email, string role, string pseudo);
        string GenerateToken(string email, string role, string pseudo);
        ClaimsPrincipal GetPrincipal(string token);
        IDictionary<string, string> ValidateToken(string token);
        string GetTokenFromRequest(HttpRequestMessage request);
    }
}