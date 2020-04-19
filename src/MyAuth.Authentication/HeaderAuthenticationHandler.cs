using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyAuth.Authentication
{
    class HeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public HeaderAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            var authHeader = Request.Headers["Authorization"];
            if (!AuthenticationHeaderValue.TryParse(authHeader, out var authVal) ||
                authVal.Scheme != MyAuthAuthenticationDefinitions.AuthenticationSchemeV1)
                return Task.FromResult(AuthenticateResult.NoResult());

            MyAuthClaims claims;

            try
            {
                claims = MyAuthClaims.Deserialize(authVal.Parameter);
            }
            catch (FormatException)
            {
                return Task.FromResult(AuthenticateResult.Fail("Authentication data has invalid format"));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
