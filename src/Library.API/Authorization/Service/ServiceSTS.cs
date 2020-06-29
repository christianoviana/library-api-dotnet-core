using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library.API.Authorization.Service
{
    public class ServiceSTS : IServiceSTS
    {
        public IHttpClientFactory ClientFactory { get; }

        public ServiceSTS(IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
        }

        public AuthenticationTicket GetTicket(string token)
        {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token);
            var lstClaims = jwtSecurityToken.Claims.ToList();

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(lstClaims, "Bearer");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var ticket = new AuthenticationTicket(claimsPrincipal, "Bearer");

            return ticket;
        }

        public async Task<bool> IsValidTokenAsync(string token)
        {
            HttpClient httpClient = this.ClientFactory.CreateClient("sts");
            httpClient.DefaultRequestHeaders.Add("contentType", "application/json");
            var _token = new { token = token };
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("login/validate", _token);
                        
            // Log in file
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return await Task.FromResult(true);

            return await Task.FromResult(false);
        }
    }
}
