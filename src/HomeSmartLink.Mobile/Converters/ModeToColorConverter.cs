using System.Globalization;

namespace HomeSmartLink.Mobile.Converters;

public class ModeToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int selectedMode || parameter is not string paramStr)
            return Microsoft.Maui.Controls.Application.Current?.Resources["SurfaceVariant"] ?? Colors.Gray;

        if (!int.TryParse(paramStr, out var buttonMode))
            return Microsoft.Maui.Controls.Application.Current?.Resources["SurfaceVariant"] ?? Colors.Gray;

        return selectedMode == buttonMode
            ? Microsoft.Maui.Controls.Application.Current?.Resources["Primary"] ?? Colors.Purple
            : Microsoft.Maui.Controls.Application.Current?.Resources["SurfaceVariant"] ?? Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
