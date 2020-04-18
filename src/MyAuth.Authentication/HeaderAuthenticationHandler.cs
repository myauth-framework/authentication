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
                authVal.Scheme != HeaderBasedDefinitions.AuthenticationSchemeV1)
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authVal.Parameter),
                new Claim(ClaimTypes.Name, authVal.Parameter)
            };

            var claimsHeader = Request.Headers[HeaderBasedDefinitions.UserClaimsHeaderName];
            if (!string.IsNullOrWhiteSpace(claimsHeader))
            {
                try
                {
                    var claimsHeaderPayload = JwtPayload.Deserialize(claimsHeader);

                    var decodedClaims = claimsHeaderPayload.Claims
                        .Select(c => new Claim(
                                NormalizeClaimType(c.Type), 
                                HttpUtility.UrlDecode(c.Value), 
                                c.ValueType))
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

        private string NormalizeClaimType(string claimType)
        {
            switch (claimType.ToLower())
            {
                case "role":
                case "roles":
                    return ClaimTypes.Role;
                default:
                    return claimType;
            }
        }
    }
}
