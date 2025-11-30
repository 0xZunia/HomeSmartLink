using HomeSmartLink.Mobile.ViewModels;

namespace HomeSmartLink.Mobile.Views;

public partial class RoomDetailPage : ContentPage
{
    public RoomDetailPage(RoomDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
