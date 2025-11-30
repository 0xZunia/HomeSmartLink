using CommunityToolkit.Maui;
using HomeSmartLink.Application.Common.Interfaces;
using HomeSmartLink.Infrastructure.Api;
using HomeSmartLink.Mobile.Services;
using HomeSmartLink.Mobile.ViewModels;
using HomeSmartLink.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace HomeSmartLink.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services - Storage
        builder.Services.AddSingleton<ISessionStorage, MauiSessionStorage>();
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();

        // HTTP Client
        builder.Services.AddHttpClient<ISmartLinkApiClient, SmartLinkApiClient>();

        // Services - Session
        builder.Services.AddSingleton<ISessionService, SessionService>();

        // ViewModels
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddTransient<RoomDetailViewModel>();

        // Views
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddTransient<RoomDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
