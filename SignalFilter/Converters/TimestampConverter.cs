using System.Globalization;
using System.Windows.Data;

namespace SignalFilter.Converters;

[ValueConversion(typeof(ulong), typeof(string))]
public sealed class TimestampConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ulong ms)
            return DateTimeOffset.FromUnixTimeMilliseconds((long)ms)
                                 .LocalDateTime
                                 .ToString("dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
