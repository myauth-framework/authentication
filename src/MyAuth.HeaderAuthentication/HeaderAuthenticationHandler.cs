using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace MyAuth.HeaderAuthentication
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
            var userIdHeader = Request.Headers[HeaderBasedDefinitions.UserIdHeaderName];
            if (string.IsNullOrWhiteSpace(userIdHeader))
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userIdHeader),
                new Claim(ClaimTypes.Name, userIdHeader)
            };

            var claimsHeader = Request.Headers[HeaderBasedDefinitions.UserClaimsHeaderName];
            if (claimsHeader != StringValues.Empty)
            {
                try
                {
                    var claimsHeaderPayload = JwtPayload.Deserialize(claimsHeader);

                    var decodedClaims = claimsHeaderPayload.Claims
                        .Select(c =>new Claim(c.Type, HttpUtility.UrlDecode(c.Value), c.ValueType))
                        .Where(c => ClaimsBlackList.Claims.All(blc => blc != c.Type));
                        
                    claims.AddRange(decodedClaims);
                }
                catch
                {
                    var reason = $"Invalid {HeaderBasedDefinitions.UserClaimsHeaderName} Header";
                    return Task.FromResult(AuthenticateResult.Fail(reason));
                }
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
