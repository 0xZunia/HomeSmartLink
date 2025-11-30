using HomeSmartLink.Mobile.Views;

namespace HomeSmartLink.Mobile;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider serviceProvider)
    {
        try
        {
            Console.WriteLine("AppShell: Starting initialization");
            InitializeComponent();
            Console.WriteLine("AppShell: InitializeComponent done");

            // Register routes for navigation
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            Routing.RegisterRoute(nameof(RoomDetailPage), typeof(RoomDetailPage));
            Console.WriteLine("AppShell: Routes registered");

            // Set initial page from DI
            var loginPage = serviceProvider.GetRequiredService<LoginPage>();
            Console.WriteLine("AppShell: LoginPage resolved from DI");

            Items.Clear();
            Items.Add(new ShellContent
            {
                Route = "LoginPage",
                Content = loginPage
            });
            Console.WriteLine("AppShell: Shell content set");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AppShell ERROR: {ex}");
            throw;
        }
    }
}
