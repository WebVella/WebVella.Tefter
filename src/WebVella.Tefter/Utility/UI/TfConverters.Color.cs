namespace WebVella.Tefter.Utility;

public static partial class TfConverters
{
	public static System.Drawing.Color TfColorToColor(TfColor? color)
	{
		if (color is null || color == TfColor.Black)
		{
			return new System.Drawing.Color();
		}
		return System.Drawing.ColorTranslator.FromHtml(color.GetAttribute().Value);
	}

	public static System.Drawing.Color HEXToColor(string color)
	{
		if (String.IsNullOrWhiteSpace(color))
		{
			return new System.Drawing.Color();
		}
		return System.Drawing.ColorTranslator.FromHtml(color);
	}

	public static System.Drawing.Color ChangeColorBrightness(System.Drawing.Color color, float correctionFactor)
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

	public static System.Drawing.Color ChangeColorDarkness(System.Drawing.Color color, float correctionFactor)
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

	public static string ChangeColorBrightnessHex(System.Drawing.Color color, float correctionFactor)
	{
		return ColorToHex(ChangeColorBrightness(color, correctionFactor));
	}

	public static string ChangeColorDarknessHex(System.Drawing.Color color, float correctionFactor)
	{
		return ColorToHex(ChangeColorDarkness(color, correctionFactor));
	}

	public static String ColorToHex(System.Drawing.Color c)
		=> $"#{c.R:X2}{c.G:X2}{c.B:X2}";

	public static String ColorToRGB(System.Drawing.Color c)
		=> $"RGB({c.R},{c.G},{c.B})";

	public static string GetCssColorFromString(string colorString)
	{
		if (String.IsNullOrWhiteSpace(colorString)) return null;
		colorString = colorString.Trim().ToLowerInvariant();
		//Check if css code color
		if (colorString.StartsWith("#") || colorString.StartsWith("rgb")
		|| colorString.StartsWith("hsl") || colorString.StartsWith("hwb"))
			return colorString;

		//Check if TfColor int
		if (int.TryParse(colorString, out int value)
		&& Enum.IsDefined(typeof(TfColor), value))
		{
			return ((TfColor)value).ToAttributeValue();
		}
		//Check if TfColor string
		if (Enum.TryParse<TfColor>(colorString, true, out TfColor outColor))
		{
			return outColor.ToAttributeValue();
		}
		//named color
		return colorString;
	}
}
