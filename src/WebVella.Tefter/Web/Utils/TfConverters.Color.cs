namespace WebVella.Tefter.Web.Utils;

internal static partial class TfConverters
{
	internal static System.Drawing.Color OfficeColorToColor(OfficeColor? color)
	{
		if (color is null || color == OfficeColor.Default)
		{
			return new System.Drawing.Color();
		}
		return System.Drawing.ColorTranslator.FromHtml(color.ToAttributeValue());
	}

	internal static System.Drawing.Color HEXToColor(string color)
	{
		if (String.IsNullOrWhiteSpace(color))
		{
			return new System.Drawing.Color();
		}
		return System.Drawing.ColorTranslator.FromHtml(color);
	}

	internal static System.Drawing.Color ChangeColorBrightness(System.Drawing.Color color, float correctionFactor)
	{
		float red = (float)color.R;
		float green = (float)color.G;
		float blue = (float)color.B;

		if (correctionFactor < 0)
		{
			correctionFactor = 1 + correctionFactor;
			red *= correctionFactor;
			green *= correctionFactor;
			blue *= correctionFactor;
		}
		else
		{
			red = (255 - red) * correctionFactor + red;
			green = (255 - green) * correctionFactor + green;
			blue = (255 - blue) * correctionFactor + blue;
		}

		if (red > 255) red = 255;
		if (red < 0) red = 0;
		if (green > 255) green = 255;
		if (green < 0) green = 0;
		if (blue > 255) blue = 255;
		if (blue < 0) blue = 0;

		return System.Drawing.Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
	}

	internal static System.Drawing.Color ChangeColorDarkness(System.Drawing.Color color, float correctionFactor)
	{
		float red = (float)color.R;
		float green = (float)color.G;
		float blue = (float)color.B;

		if (correctionFactor < 0)
		{
			correctionFactor = -1 + correctionFactor;
			red *= correctionFactor;
			green *= correctionFactor;
			blue *= correctionFactor;
		}
		else
		{
			red = (255 - red) * correctionFactor + red;
			green = (255 - green) * correctionFactor + green;
			blue = (255 - blue) * correctionFactor + blue;
		}

		if (red > 255) red = 255;
		if (red < 0) red = 0;
		if (green > 255) green = 255;
		if (green < 0) green = 0;
		if (blue > 255) blue = 255;
		if (blue < 0) blue = 0;

		return System.Drawing.Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
	}

	internal static string ChangeColorBrightnessHex(System.Drawing.Color color, float correctionFactor)
	{
		return ColorToHex(ChangeColorBrightness(color, correctionFactor));
	}

	internal static string ChangeColorDarknessHex(System.Drawing.Color color, float correctionFactor)
	{
		return ColorToHex(ChangeColorDarkness(color, correctionFactor));
	}

	internal static String ColorToHex(System.Drawing.Color c)
		=> $"#{c.R:X2}{c.G:X2}{c.B:X2}";

	internal static String ColorToRGB(System.Drawing.Color c)
		=> $"RGB({c.R},{c.G},{c.B})";

	internal static string GetCssColorFromString(string colorString)
	{
		if (String.IsNullOrWhiteSpace(colorString)) return null;
		colorString = colorString.Trim().ToLowerInvariant();
		//Check if css code color
		if (colorString.StartsWith("#") || colorString.StartsWith("rgb")
		|| colorString.StartsWith("hsl") || colorString.StartsWith("hwb"))
			return colorString;

		//Check if OfficeColor int
		if (int.TryParse(colorString, out int value)
		&& Enum.IsDefined(typeof(OfficeColor), value))
		{
			return ((OfficeColor)value).ToAttributeValue();
		}
		//Check if OfficeColor string
		if (Enum.TryParse<OfficeColor>(colorString, true, out OfficeColor outColor))
		{
			return outColor.ToAttributeValue();
		}
		//named color
		return colorString;
	}
}
