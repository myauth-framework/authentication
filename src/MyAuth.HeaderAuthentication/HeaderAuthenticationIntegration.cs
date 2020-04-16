using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MyAuth.HeaderAuthentication
{
    /// <summary>
    /// Contains extension methods for Identity abilities integration
    /// </summary>
    public static class HeaderAuthenticationIntegration
    {
        public static IServiceCollection AddHeaderAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(HeaderBasedDefinitions.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>(
                    HeaderBasedDefinitions.AuthenticationScheme, null);

            return services;
        }
    }
}
