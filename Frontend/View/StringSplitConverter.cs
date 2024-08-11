using System;
using System.Globalization;
using System.Windows.Data;

namespace Frontend.View
{
    public class StringSplitConverter : IValueConverter
    {
        public int PartIndex { get; set; } // Index of the part to return

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                var parts = str.Split(':');
                if (PartIndex >= 0 && PartIndex < parts.Length)
                {
                    return parts[PartIndex];
                }
            }
            return string.Empty; // Fallback if something goes wrong
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}