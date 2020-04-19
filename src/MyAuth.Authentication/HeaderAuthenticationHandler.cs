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
using MyLab.LogDsl;

namespace MyAuth.Authentication
{
    class HeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DslLogger _log;

        public HeaderAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _log = logger.CreateLogger<HeaderAuthenticationHandler>().Dsl();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"];
            if (!AuthenticationHeaderValue.TryParse(authHeader, out var authVal))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (authVal.Scheme != MyAuthAuthenticationDefinitions.AuthenticationSchemeV1)
            {
                _log.Act("Unexpected auth scheme detected")
                    .AndFactIs("Auth header", authHeader)
                    .AndMarkAs("warning")
                    .AndMarkAs("auth")
                    .Write();
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            
            MyAuthClaims claims;

            try
            {
                claims = MyAuthClaims.Deserialize(authVal.Parameter);
            }
            catch (FormatException e)
            {
                _log.Error("Authentication data has invalid format", e)
                    .AndMarkAs("auth")
                    .Write();

                return Task.FromResult(AuthenticateResult.Fail("Authentication data has invalid format"));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
