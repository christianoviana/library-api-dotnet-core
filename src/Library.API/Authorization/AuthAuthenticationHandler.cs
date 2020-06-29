
using Library.API.Authorization.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Library.API.Middleware.Auth
{
    public class AuthAuthenticationHandler : AuthenticationHandler<AuthAuthenticationOptions>
    {
        private readonly IOptionsMonitor<AuthAuthenticationOptions> options;
        private IServiceSTS ServiceSTS { get; }

        public AuthAuthenticationHandler(IOptionsMonitor<AuthAuthenticationOptions> options,
                                         IServiceSTS serviceSTS,
                                         ILoggerFactory logger, UrlEncoder encoder, 
                                         ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this.options = options;
            this.ServiceSTS = serviceSTS;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue authenticationHeaderValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!authenticationHeaderValue.Scheme.Equals(options.CurrentValue.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                //Not supported Schema
                return AuthenticateResult.NoResult();
            }

            try
            {
                // Validate the user request token
                bool isValidToken = await ServiceSTS.IsValidTokenAsync(authenticationHeaderValue.Parameter);

                if (isValidToken)
                    return AuthenticateResult.Success(ServiceSTS.GetTicket(authenticationHeaderValue.Parameter));
                else
                    return AuthenticateResult.Fail("This token is invalid or expirated");
            }
            catch (Exception)
            {
                // log the error
                return AuthenticateResult.Fail("Occur an error validating this token.");
            }
               
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Security Token Service, charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
