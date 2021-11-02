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
using MyLab.Log.Dsl;

namespace MyAuth.Authentication
{
    class MyAuth1AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IDslLogger _log;

        public MyAuth1AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory coreLoggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IDslLogger<MyAuth1AuthenticationHandler> logger = null)
            : base(options, coreLoggerFactory, encoder, clock)
        {
            _log = logger;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!SchemeDetector.IsSchema1(Request.Headers, out var authVal))
                return Task.FromResult(AuthenticateResult.NoResult());

            MyAuth1Claims claims;

            try
            {
                claims = MyAuth1Claims.Deserialize(authVal.Parameter);
            }
            catch (FormatException e)
            {
                _log?.Error("Authentication data has invalid format", e)
                    .AndLabel("auth")
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
