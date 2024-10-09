using System;
using System.Globalization;
using System.Windows.Data;
using ZusiMeter;
using ZusiMeterGaugesLib.Extensions;

namespace ZusiMeter.Converter
{
    [ValueConversion(typeof(bool), typeof(double))]

    public class StatusToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return 0.0;
            }
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
