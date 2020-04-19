using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MyAuth.Authentication
{
    /// <summary>
    /// Contains extension methods for Identity abilities integration
    /// </summary>
    public static class HeaderAuthenticationIntegration
    {
        public static IServiceCollection AddMyAuthAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(MyAuthAuthenticationDefinitions.AuthenticationSchemeV1)
                .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>(
                    MyAuthAuthenticationDefinitions.AuthenticationSchemeV1, null);

            return services;
        }

        public static HttpClient MyAuth1Authentication(this HttpClient client, string userId, IEnumerable<Claim> claims = null)
        {
            var rc = new List<Claim> {new Claim(ClaimTypes.NameIdentifier, userId)};
            if(claims != null)
                rc.AddRange(claims);

            return MyAuth1Authentication(client, rc);
        }

        public static HttpClient MyAuth1Authentication(this HttpClient client, IEnumerable<Claim> claims)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                MyAuthAuthenticationDefinitions.AuthenticationSchemeV1,
                new MyAuthClaims(claims).Serialize()
            );

            return client;
        }
    }
}
