using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSmartLink.Application.Common.Interfaces;
using HomeSmartLink.Mobile.Views;

namespace HomeSmartLink.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly ISmartLinkApiClient _apiClient;
    private readonly ISessionService _sessionService;
    private readonly ISessionStorage _sessionStorage;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _rememberMe = true;

    public LoginViewModel(
        ISmartLinkApiClient apiClient,
        ISessionService sessionService,
        ISessionStorage sessionStorage,
        IServiceProvider serviceProvider)
    {
        _apiClient = apiClient;
        _sessionService = sessionService;
        _sessionStorage = sessionStorage;
        _serviceProvider = serviceProvider;
        Title = "Connexion";
    }

    [RelayCommand]
    private async Task CheckExistingSessionAsync()
    {
        if (await _sessionService.HasValidSessionAsync())
        {
            var sessionToken = await _sessionService.GetSessionTokenAsync();
            var homeId = await _sessionService.GetHomeIdAsync();

            if (!string.IsNullOrEmpty(sessionToken) && !string.IsNullOrEmpty(homeId))
            {
                _apiClient.Session = sessionToken;
                await NavigateToDashboardAsync();
            }
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez entrer votre email et mot de passe";
            return;
        }

        await ExecuteAsync(async () =>
        {
            // Login
            var loginResult = await _apiClient.SignInAsync(Email, Password);

            if (!loginResult.IsSuccess || loginResult.Data == null)
            {
                ErrorMessage = loginResult.ErrorMessage ?? "Email ou mot de passe incorrect";
                return;
            }

            // Get invitations to find home ID
            var invitationsResult = await _apiClient.FetchMyInvitationsAsync();
            if (!invitationsResult.IsSuccess || invitationsResult.Data == null || invitationsResult.Data.Count == 0)
            {
                ErrorMessage = "Aucune maison trouvee";
                return;
            }

            var homeId = invitationsResult.Data[0].HomeId;

            // Save session
            if (RememberMe && !string.IsNullOrEmpty(loginResult.Data.Token))
            {
                await _sessionService.SaveSessionAsync(loginResult.Data.Token, Email, homeId);
            }

            await NavigateToDashboardAsync();
        });
    }

    private async Task NavigateToDashboardAsync()
    {
        var dashboardPage = _serviceProvider.GetRequiredService<DashboardPage>();
        if (Microsoft.Maui.Controls.Application.Current?.MainPage is NavigationPage navPage)
        {
            await navPage.PushAsync(dashboardPage);
        }
    }
}
