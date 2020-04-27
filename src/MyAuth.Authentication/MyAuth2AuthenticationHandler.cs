using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.LogDsl;

namespace MyAuth.Authentication
{
    class MyAuth2AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DslLogger _log;

        public MyAuth2AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _log = logger.CreateLogger<MyAuth1AuthenticationHandler>().Dsl();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!SchemeDetector.IsSchema2(Request.Headers))
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = MyAuth2Claims.LoadFomHeaders(Request.Headers);
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
