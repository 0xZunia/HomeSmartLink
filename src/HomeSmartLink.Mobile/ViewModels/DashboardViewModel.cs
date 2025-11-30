using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSmartLink.Application.Common.Interfaces;
using HomeSmartLink.Application.Common.Models;
using HomeSmartLink.Mobile.Views;

namespace HomeSmartLink.Mobile.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly ISmartLinkApiClient _apiClient;
    private readonly ISessionService _sessionService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = [];

    [ObservableProperty]
    private string _homeName = "Ma Maison";

    [ObservableProperty]
    private int _totalDevices;

    [ObservableProperty]
    private int _activeDevices;

    [ObservableProperty]
    private double _averageTemperature;

    [ObservableProperty]
    private int _totalHeatingHours;

    [ObservableProperty]
    private bool _isRefreshing;

    public DashboardViewModel(ISmartLinkApiClient apiClient, ISessionService sessionService, IServiceProvider serviceProvider)
    {
        _apiClient = apiClient;
        _sessionService = sessionService;
        _serviceProvider = serviceProvider;
        Title = "Dashboard";
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var homeId = await _sessionService.GetHomeIdAsync();
            if (string.IsNullOrEmpty(homeId))
            {
                await NavigateToLoginAsync();
                return;
            }

            // Get home status
            var statusResult = await _apiClient.FetchDeviceDataAsync(homeId);
            if (!statusResult.IsSuccess || statusResult.Data == null)
            {
                ErrorMessage = statusResult.ErrorMessage ?? "Impossible de charger les donnees";
                return;
            }

            var statusList = statusResult.Data;

            // Get home info for room names
            var homeResult = await _apiClient.FetchHomeAsync(homeId);
            var homeData = homeResult.Data;

            // Group by room
            var roomGroups = statusList
                .GroupBy(s => s.RoomId)
                .Select(g =>
                {
                    var roomInfo = homeData?.Rooms.FirstOrDefault(r => r.Position == g.Key);
                    return new RoomDto
                    {
                        Position = g.Key,
                        Name = roomInfo?.Name ?? $"Piece {g.Key}",
                        Area = roomInfo?.Area ?? 0,
                        Settings = roomInfo?.Settings ?? new HeaterSettingsDto(),
                        Devices = g.ToList()
                    };
                })
                .ToList();

            Rooms = new ObservableCollection<RoomDto>(roomGroups);

            // Calculate stats
            TotalDevices = statusList.Count;
            ActiveDevices = statusList.Count(d => d.Mode != 1 && d.Mode != 2);

            if (statusList.Any())
            {
                AverageTemperature = Math.Round(statusList.Average(d => d.Ambient), 1);
                TotalHeatingHours = statusList.Sum(d => d.DailyHours);
            }

            // Get home name
            if (homeData != null)
            {
                var invResult = await _apiClient.FetchMyInvitationsAsync();
                if (invResult.IsSuccess && invResult.Data?.Count > 0)
                {
                    HomeName = invResult.Data[0].HomeName ?? "Ma Maison";
                }
            }
        });

        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task NavigateToRoomAsync(RoomDto room)
    {
        if (room == null) return;

        var roomDetailPage = _serviceProvider.GetRequiredService<RoomDetailPage>();
        var viewModel = roomDetailPage.BindingContext as RoomDetailViewModel;
        viewModel?.SetRoom(room);

        if (Microsoft.Maui.Controls.Application.Current?.MainPage is NavigationPage navPage)
        {
            await navPage.PushAsync(roomDetailPage);
        }
    }

    [RelayCommand]
    private async Task SetAllRoomsModeAsync(object modeParam)
    {
        if (!int.TryParse(modeParam?.ToString(), out var mode))
            return;

        await ExecuteAsync(async () =>
        {
            var homeId = await _sessionService.GetHomeIdAsync();
            if (string.IsNullOrEmpty(homeId)) return;

            // Get home to modify
            var homeResult = await _apiClient.FetchHomeAsync(homeId);
            if (!homeResult.IsSuccess || homeResult.Data == null) return;

            // Update all room modes
            foreach (var room in homeResult.Data.Rooms)
            {
                room.Settings.Mode = mode;
            }

            // Save home
            await _apiClient.SaveHomeAsync(homeResult.Data);

            await LoadDataAsync();
        });
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _sessionService.ClearSession();
        _apiClient.Logout();
        await NavigateToLoginAsync();
    }

    private async Task NavigateToLoginAsync()
    {
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        if (Microsoft.Maui.Controls.Application.Current?.MainPage is NavigationPage navPage)
        {
            navPage.Navigation.InsertPageBefore(loginPage, navPage.CurrentPage);
            await navPage.PopToRootAsync();
        }
    }
}
