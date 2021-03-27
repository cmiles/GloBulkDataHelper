using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace GloBulkDataHelper.WinDesktopViewer.VisibleStatus
{
    public class ProgressVisibilityConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            return value switch
            {
                VisibleStatusState when Enum.TryParse<VisibleStatusState>(value.ToString(), true, out var translateEnum)
                    => translateEnum == VisibleStatusState.ShowProgress ? Visibility.Visible : Visibility.Collapsed,
                _ => Visibility.Collapsed
            };
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}