using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSmartLink.Application.Common.Interfaces;
using HomeSmartLink.Application.Common.Models;

namespace HomeSmartLink.Mobile.ViewModels;

public partial class RoomDetailViewModel : BaseViewModel
{
    private readonly ISmartLinkApiClient _apiClient;
    private readonly ISessionService _sessionService;

    [ObservableProperty]
    private RoomDto? _room;

    public void SetRoom(RoomDto room) => Room = room;

    [ObservableProperty]
    private ObservableCollection<DeviceDataDto> _devices = [];

    [ObservableProperty]
    private int _selectedMode;

    [ObservableProperty]
    private double _manualSetpoint = 20.0;

    [ObservableProperty]
    private double _ecoSetpoint = 17.0;

    [ObservableProperty]
    private double _comfortSetpoint = 20.0;

    [ObservableProperty]
    private double _averageTemperature;

    [ObservableProperty]
    private double _averageSetpoint;

    [ObservableProperty]
    private string _heatingStatus = "Inconnu";

    [ObservableProperty]
    private Color _heatingStatusColor = Colors.Gray;

    public string[] ModeNames { get; } = ["Auto", "Arret", "Hors-gel", "Manuel", "Programme"];

    public RoomDetailViewModel(ISmartLinkApiClient apiClient, ISessionService sessionService)
    {
        _apiClient = apiClient;
        _sessionService = sessionService;
    }

    partial void OnRoomChanged(RoomDto? value)
    {
        if (value != null)
        {
            Title = value.Name;
            Devices = new ObservableCollection<DeviceDataDto>(value.Devices);

            // Load setpoints from settings
            EcoSetpoint = value.Settings.EconomyValue;
            ComfortSetpoint = value.Settings.StandardValue;
            ManualSetpoint = value.Settings.ManualValue;
            SelectedMode = value.Settings.Mode;

            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        if (Devices.Count == 0) return;

        AverageTemperature = Math.Round(Devices.Average(d => d.Ambient), 1);
        AverageSetpoint = Math.Round(Devices.Average(d => d.Setpoint), 1);

        // Get mode from first device
        var firstDevice = Devices.FirstOrDefault();
        if (firstDevice != null)
        {
            SelectedMode = firstDevice.Mode;
            ManualSetpoint = firstDevice.Setpoint;
        }

        // Determine heating status
        var tempDiff = AverageSetpoint - AverageTemperature;
        if (SelectedMode == 1)
        {
            HeatingStatus = "Arret";
            HeatingStatusColor = Colors.Gray;
        }
        else if (SelectedMode == 2)
        {
            HeatingStatus = "Hors-gel";
            HeatingStatusColor = Colors.LightBlue;
        }
        else if (tempDiff > 0.5)
        {
            HeatingStatus = "En chauffe";
            HeatingStatusColor = Color.FromArgb("#FF5722");
        }
        else if (tempDiff > 0)
        {
            HeatingStatus = "Maintien";
            HeatingStatusColor = Color.FromArgb("#FFC107");
        }
        else
        {
            HeatingStatus = "Temperature atteinte";
            HeatingStatusColor = Color.FromArgb("#4CAF50");
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (Room == null) return;

        try
        {
            IsRefreshing = true;
            var homeId = await _sessionService.GetHomeIdAsync();
            if (string.IsNullOrEmpty(homeId)) return;

            var statusResult = await _apiClient.FetchDeviceDataAsync(homeId);
            if (statusResult.IsSuccess && statusResult.Data != null)
            {
                var roomDevices = statusResult.Data.Where(d => d.RoomId == Room.RoomId).ToList();
                Devices = new ObservableCollection<DeviceDataDto>(roomDevices);
                Room = Room with { Devices = roomDevices };
                UpdateStats();
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task SetModeAsync(object modeParam)
    {
        if (Room == null || !int.TryParse(modeParam?.ToString(), out var mode)) return;

        await ExecuteAsync(async () =>
        {
            var homeId = await _sessionService.GetHomeIdAsync();
            if (string.IsNullOrEmpty(homeId)) return;

            // Get home to modify
            var homeResult = await _apiClient.FetchHomeAsync(homeId);
            if (!homeResult.IsSuccess || homeResult.Data == null) return;

            // Find and update room
            var roomToUpdate = homeResult.Data.Rooms.FirstOrDefault(r => r.Position == Room.RoomId);
            if (roomToUpdate != null)
            {
                roomToUpdate.Settings.Mode = mode;
                await _apiClient.SaveHomeAsync(homeResult.Data);
                SelectedMode = mode;
            }

            await RefreshAsync();
        });
    }

    [RelayCommand]
    private async Task SetManualTemperatureAsync()
    {
        if (Room == null) return;

        await ExecuteAsync(async () =>
        {
            var homeId = await _sessionService.GetHomeIdAsync();
            if (string.IsNullOrEmpty(homeId)) return;

            // Get home to modify
            var homeResult = await _apiClient.FetchHomeAsync(homeId);
            if (!homeResult.IsSuccess || homeResult.Data == null) return;

            // Find and update room
            var roomToUpdate = homeResult.Data.Rooms.FirstOrDefault(r => r.Position == Room.RoomId);
            if (roomToUpdate != null)
            {
                roomToUpdate.Settings.Mode = 3; // Manual mode
                roomToUpdate.Settings.ManualValue = ManualSetpoint;
                await _apiClient.SaveHomeAsync(homeResult.Data);
                SelectedMode = 3;
            }

            await RefreshAsync();
        });
    }

    [RelayCommand]
    private async Task SetEcoComfortTemperaturesAsync()
    {
        if (Room == null) return;

        await ExecuteAsync(async () =>
        {
            var homeId = await _sessionService.GetHomeIdAsync();
            if (string.IsNullOrEmpty(homeId)) return;

            // Get home to modify
            var homeResult = await _apiClient.FetchHomeAsync(homeId);
            if (!homeResult.IsSuccess || homeResult.Data == null) return;

            // Find and update room
            var roomToUpdate = homeResult.Data.Rooms.FirstOrDefault(r => r.Position == Room.RoomId);
            if (roomToUpdate != null)
            {
                roomToUpdate.Settings.EconomyValue = EcoSetpoint;
                roomToUpdate.Settings.StandardValue = ComfortSetpoint;
                await _apiClient.SaveHomeAsync(homeResult.Data);
            }

            await RefreshAsync();
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        if (Microsoft.Maui.Controls.Application.Current?.MainPage is NavigationPage navPage)
        {
            await navPage.PopAsync();
        }
    }
}
