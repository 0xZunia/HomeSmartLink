using HomeSmartLink.Application.Common.Interfaces;
using HomeSmartLink.Infrastructure.Api;
using HomeSmartLink.Infrastructure.Identity;
using HomeSmartLink.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeSmartLink.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // HTTP Client
        services.AddHttpClient<ISmartLinkApiClient, SmartLinkApiClient>(client =>
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Services
        services.AddScoped<ISessionStorage, BrowserSessionStorage>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
