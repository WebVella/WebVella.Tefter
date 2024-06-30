namespace WebVella.Tefter.Web.Utils;

internal static class RenderUtils
{
	private static string conversionPrefix = "tf-";

	internal static string ConvertGuidToHtmlElementId(Guid? guid)
	{
		if (guid == null) return null;
		return $"{conversionPrefix}{guid}";
	}

	internal static Guid? ConvertHtmlElementIdToGuid(string htmlId)
	{
		if (String.IsNullOrWhiteSpace(htmlId)) return null;
		var match = htmlId.Trim().ToLowerInvariant();
		if (!match.StartsWith(conversionPrefix)) return null;

		match = match.Replace(conversionPrefix, "");

		if (Guid.TryParse(match, out Guid id)) return id;

		return null;
	}

	internal static string StringOverflow(string input, int charCount)
	{
		if (input.Length <= charCount) return input;
		return input.Substring(0, charCount) + "...";
	}

	internal static int CalcSkip(int pageSize, int page) => (page - 1) * pageSize;

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


	internal static string GetUserInitions(TucUser user)
	{
		var list = new List<string>();
		if (!String.IsNullOrWhiteSpace(user.FirstName))
		{
			list.Add(user.FirstName.Substring(0, 1));
		}
		if (!String.IsNullOrWhiteSpace(user.LastName))
		{
			list.Add(user.LastName.Substring(0, 1));
		}

		if (list.Count == 0) return "?";

		return String.Join("", list).ToUpperInvariant();
	}

}
