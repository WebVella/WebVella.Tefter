namespace WebVella.Tefter.Web.Utils;

internal static class TfConverters
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

	internal static int CalcSkip(int page, int pageSize) => (page - 1) * pageSize;

	internal static System.Drawing.Color OfficeColorToColor(OfficeColor? color)
	{
		if (color is null || color == OfficeColor.Default)
		{
			return new System.Drawing.Color();
		}
		return System.Drawing.ColorTranslator.FromHtml(color.ToAttributeValue());
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

	internal static List<string> GetSpaceIconLibrary()
	{
		var result = new List<string>();
		foreach (var item in Icons.AllIcons)
		{
			if (item.Size == TfConstants.IconSize
			&& item.Variant == TfConstants.IconVariant
			&& item.Name.Length <= 5)
				result.Add(item.Name);
		}
		return result.Take(50).ToList();
	}

	internal static List<string> GetUniqueTagsFromText(
		string text)
	{
		var result = new List<string>();

		if (string.IsNullOrWhiteSpace(text))
			return result;

		var regex = new Regex(@"#\w+");
		var matches = regex.Matches(text);
		foreach (var match in matches)
		{
			var tag = match.ToString().ToLowerInvariant().Trim().Substring(1);
			if (!string.IsNullOrWhiteSpace(tag) && !result.Contains(tag))
				result.Add(tag);
		}

		return result;
	}

	internal static T Convert<T>(string input)
	{
		try
		{
			var converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter != null)
			{
				// Cast ConvertFromString(string text) : object to (T)
				return (T)converter.ConvertFromString(input);
			}
			return default(T);
		}
		catch (NotSupportedException)
		{
			return default(T);
		}
	}

	internal static bool IsValidEmail(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
			return false;

		try
		{
			// Normalize the domain
			email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
								  RegexOptions.None, TimeSpan.FromMilliseconds(200));

			// Examines the domain part of the email and normalizes it.
			string DomainMapper(Match match)
			{
				// Use IdnMapping class to convert Unicode domain names.
				var idn = new IdnMapping();

				// Pull out and process domain name (throws ArgumentException on invalid)
				string domainName = idn.GetAscii(match.Groups[2].Value);

				return match.Groups[1].Value + domainName;
			}
		}
		catch (RegexMatchTimeoutException e)
		{
			return false;
		}
		catch (ArgumentException e)
		{
			return false;
		}

		try
		{
			return Regex.IsMatch(email,
				@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
				RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
	}

	internal static bool IsValidURL(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
			return false;

		try
		{
			Uri uriResult;
			return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
				&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		}
		catch (ArgumentException e)
		{
			return false;
		}
	}
}
