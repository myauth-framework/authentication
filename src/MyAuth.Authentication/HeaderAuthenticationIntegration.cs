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
            services.AddAuthentication(HeaderBasedDefinitions.AuthenticationSchemeV1)
                .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>(
                    HeaderBasedDefinitions.AuthenticationSchemeV1, null);

            return services;
        }
    }
}
