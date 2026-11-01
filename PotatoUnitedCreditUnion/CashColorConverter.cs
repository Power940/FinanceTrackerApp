using System.Globalization;

namespace PotatoUnitedCreditUnion;

public class CashColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string cash = value?.ToString() ?? "";

        if (cash.Contains('+'))
            return Colors.Green;

        if (cash.Contains('-'))
            return Colors.Red;

        return Colors.Black;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}