using Microsoft.AspNetCore.Authentication;

namespace Library.API.Middleware.Auth
{
    public class AuthAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Scheme { get; set; }

        public AuthAuthenticationOptions()
        {
        }
    }
}
