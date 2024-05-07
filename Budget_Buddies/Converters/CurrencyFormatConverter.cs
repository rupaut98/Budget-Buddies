using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Budget_Buddies.Converters
{
    public class CurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue && parameter is string currencySymbol)
            {
                // Formats the decimal value as a currency string with two decimal places.
                return $"{currencySymbol}{decimalValue:N2}";
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Converting back is not supported.");
        }
    }
}