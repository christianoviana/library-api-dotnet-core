using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
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

            var content = new StringContent(JsonSerializer.Serialize(_token));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await httpClient.PostAsync("login/validate", content);
                        
            // Log in file
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return await Task.FromResult(true);

            return await Task.FromResult(false);
        }
    }
}
