using HomeSmartLink.Mobile.Views;

namespace HomeSmartLink.Mobile;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        var navPage = new NavigationPage(loginPage)
        {
            BarBackgroundColor = Color.FromArgb("#121212"),
            BarTextColor = Colors.White
        };
        NavigationPage.SetHasNavigationBar(loginPage, false);
        return new Window(navPage);
    }
}
