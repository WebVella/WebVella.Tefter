namespace WebVella.Tefter.Web.Utils;

internal static partial class TfConverters
{
	private static string conversionPrefix = "tf-";
	private static List<string> _allIcons = null;

	internal static string ConvertGuidToHtmlElementId(Guid? guid, string prefix = "")
	{
		if (guid == null) return null;
		return $"{conversionPrefix}{(String.IsNullOrWhiteSpace(prefix) ? "" : $"{prefix}-")}{guid}";
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


	internal static string GetUserInitials(TucUser user)
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
		if (_allIcons is not null) return _allIcons;

		var result = new List<string>();
		foreach (var item in Icons.AllIcons)
		{
			if (item.Size == TfConstants.IconSize
			&& item.Variant == TfConstants.IconVariant)
				result.Add(item.Name);
		}
		_allIcons = result.ToList();
		return _allIcons;
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


	public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>
	   (Dictionary<TKey, TValue> original) where TValue : ICloneable
	{
		Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
																original.Comparer);
		foreach (KeyValuePair<TKey, TValue> entry in original)
		{
			ret.Add(entry.Key, (TValue)entry.Value.Clone());
		}
		return ret;
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
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
		catch (ArgumentException)
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
		catch (ArgumentException)
		{
			return false;
		}
	}

	internal static string GenerateDbNameFromText(string text)
	{
		if (String.IsNullOrEmpty(text)) return text;
		text = text.Trim().ToLowerInvariant();
		text = text.Replace("№", "no");
		Regex rgx = new Regex("[^a-zA-Z0-9]");
		text = rgx.Replace(text, "_");
		text = Regex.Replace(text, @"_+", "_");
		return text;
	}

	internal static string Slugify(this string phrase)
	{
		string str = phrase.RemoveDiacritics().ToLower();
		// invalid chars           
		str = Regex.Replace(str, @"[^a-z0-9\s-\p{IsCyrillic}]", "");
		// convert multiple spaces into one space   
		str = Regex.Replace(str, @"\s+", " ").Trim();
		// cut and trim 
		str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
		str = Regex.Replace(str, @"\s", "-"); // hyphens   
		return str;
	}

	internal static string GenerateQueryName()
	{
		return "q" + (Guid.NewGuid()).ToString().Split("-")[0];
	}


	public static string RemoveDiacritics(this string text)
	{
		var normalizedString = text.Normalize(NormalizationForm.FormD);
		var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

		for (int i = 0; i < normalizedString.Length; i++)
		{
			char c = normalizedString[i];
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
			}
		}

		return stringBuilder
			.ToString()
			.Normalize(NormalizationForm.FormC);
	}

	public static TItem FindItemByMatch<TItem>(this IEnumerable<TItem> items, Func<TItem,bool> matcher, Func<TItem,IEnumerable<TItem>> childGetter) where TItem : class
	{
		if (items == null)
		{
			return null;
		}

		foreach (var item in items)
		{
			if (matcher(item))
			{
				return item;
			}

			var nestedItem = FindItemByMatch(childGetter(item), matcher, childGetter);
			if (nestedItem != null)
			{
				return nestedItem;
			}
		}

		return null;
	}
}
