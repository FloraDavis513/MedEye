using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace MedEye.Converters;

public class TextSizeConverter : IValueConverter
{
	public static readonly TextSizeConverter Instance = new();
	private const double BASE_WIDTH = 1920;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if(value is double originalSize && parameter is string targetSize
										&& targetType.IsAssignableTo(typeof(double)))
		{
			switch (targetSize)
			{
				case "H1":
					return (int) TextSizes.H1 * (originalSize / BASE_WIDTH);
				case "H2":
					return (int) TextSizes.H2 * (originalSize / BASE_WIDTH);
				case "H3":
					return (int) TextSizes.H3 * (originalSize / BASE_WIDTH);
				case "H4":
					return (int) TextSizes.H4 * (originalSize / BASE_WIDTH);
				case "BaseText":
					return (int) TextSizes.BaseText * (originalSize / BASE_WIDTH);
				case "ButtonWidthBase":
					return originalSize / (int) TextSizes.ButtonWidthBase;
			}

			var isParsed = int.TryParse(targetSize, out var result);
			if (isParsed)
				return result * (originalSize / BASE_WIDTH);
		}
		return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}