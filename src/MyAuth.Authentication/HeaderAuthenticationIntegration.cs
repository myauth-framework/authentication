using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
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
            services.AddAuthentication(MyAuthAuthenticationDefinitions.SchemePolicy)
                .AddPolicyScheme(MyAuthAuthenticationDefinitions.SchemePolicy, ",", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        if (SchemeDetector.IsSchema1(context.Request.Headers, out _))
                        {
                            return MyAuthAuthenticationDefinitions.SchemeV1;
                        }

                        return MyAuthAuthenticationDefinitions.SchemeV2;
                    };
                })
                .AddScheme<AuthenticationSchemeOptions, MyAuth1AuthenticationHandler>(
                    MyAuthAuthenticationDefinitions.SchemeV1, null)
                .AddScheme<AuthenticationSchemeOptions, MyAuth2AuthenticationHandler>(
                    MyAuthAuthenticationDefinitions.SchemeV2, null);

            return services;
        }

        public static HttpClient AddMyAuthAuthentication(this HttpClient client, string userId, IEnumerable<Claim> claims = null, string scheme = MyAuthAuthenticationDefinitions.SchemeV2)
        {
            var rc = new List<Claim> {new Claim(ClaimTypes.NameIdentifier, userId)};
            if(claims != null)
                rc.AddRange(claims);

            return AddMyAuthAuthentication(client, rc, scheme);
        }

        public static HttpClient AddMyAuthAuthentication(this HttpClient client, IEnumerable<Claim> claims, string scheme = MyAuthAuthenticationDefinitions.SchemeV2)
        {
            ISchemeAdder adder;

            switch (scheme)
            {
                case MyAuthAuthenticationDefinitions.SchemeV1: adder = new MyAuth1SchemeAdder(claims); break;
                case MyAuthAuthenticationDefinitions.SchemeV2: adder = new MyAuth2SchemeAdder(claims); break;
                    default: throw new IndexOutOfRangeException("Specified scheme is not supported");
            }

            adder.Add(client);

            return client;
        }
    }
}
