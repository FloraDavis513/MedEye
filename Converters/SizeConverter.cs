using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace MedEye.Converters;

public class SizeConverter : IValueConverter
{
	public static readonly SizeConverter Instance = new();
	private const double BASE_WIDTH = 1920;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is double originalSize && parameter is string targetSize
										 && targetType.IsAssignableTo(typeof(double)))
		{
			var isParsed = int.TryParse(targetSize, out var intTargetSize);
			if (isParsed)
			{
				var baseButtonWidth = 2 * (originalSize / 9);
				return baseButtonWidth / ((intTargetSize + 100f) / 100f);
			}

			switch (targetSize)
			{
				case "60%":
					return originalSize / 100 * 60;
				break;
			}

			throw new ArgumentException();
		}
		return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}