namespace WebVella.Tefter.Web.Utils;

public static partial class TfConverters
{
	private static string conversionPrefix = "tf-";
	private static List<string> _allIcons = null;

	#region << String >>
	public static string StringOverflow(string input, int charCount)
	{
		if (input.Length <= charCount) return input;
		return input.Substring(0, charCount) + "...";
	}
	public static List<string> GetUniqueTagsFromText(
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
	public static T Convert<T>(string input)
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
	public static string Slugify(this string phrase)
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
	public static string GenerateDbNameFromText(string text)
	{
		if (String.IsNullOrEmpty(text)) return text;
		text = text.Trim().ToLowerInvariant();
		text = text.Replace("№", "no");
		text = ConvertToLatin(text);
		Regex rgx = new Regex("[^a-zA-Z0-9]");
		text = rgx.Replace(text, "_");
		text = Regex.Replace(text, @"_+", "_");
		return text;
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

	public static readonly Dictionary<char, string> ConvertedLetters = new Dictionary<char, string>
	{
		{'а', "a"},
		{'б', "b"},
		{'в', "v"},
		{'г', "g"},
		{'д', "d"},
		{'е', "e"},
		{'ё', "yo"},
		{'ж', "zh"},
		{'з', "z"},
		{'и', "i"},
		{'й', "j"},
		{'к', "k"},
		{'л', "l"},
		{'м', "m"},
		{'н', "n"},
		{'о', "o"},
		{'п', "p"},
		{'р', "r"},
		{'с', "s"},
		{'т', "t"},
		{'у', "u"},
		{'ф', "f"},
		{'х', "h"},
		{'ц', "c"},
		{'ч', "ch"},
		{'ш', "sh"},
		{'щ', "sch"},
		{'ъ', "j"},
		{'ы', "i"},
		{'ь', "j"},
		{'э', "e"},
		{'ю', "yu"},
		{'я', "ya"},
		{'А', "A"},
		{'Б', "B"},
		{'В', "V"},
		{'Г', "G"},
		{'Д', "D"},
		{'Е', "E"},
		{'Ё', "Yo"},
		{'Ж', "Zh"},
		{'З', "Z"},
		{'И', "I"},
		{'Й', "J"},
		{'К', "K"},
		{'Л', "L"},
		{'М', "M"},
		{'Н', "N"},
		{'О', "O"},
		{'П', "P"},
		{'Р', "R"},
		{'С', "S"},
		{'Т', "T"},
		{'У', "U"},
		{'Ф', "F"},
		{'Х', "H"},
		{'Ц', "C"},
		{'Ч', "Ch"},
		{'Ш', "Sh"},
		{'Щ', "Sch"},
		{'Ъ', "J"},
		{'Ы', "I"},
		{'Ь', "J"},
		{'Э', "E"},
		{'Ю', "Yu"},
		{'Я', "Ya"}
	};
	public static string ConvertToLatin(string source)
	{
		var result = new StringBuilder();
		foreach (var letter in source)
		{
			if (ConvertedLetters.ContainsKey(letter))
				result.Append(ConvertedLetters[letter]);
			else
				result.Append(letter);
		}
		return result.ToString();
	}

	#endregion

	#region << Guid >>
	public static string ConvertGuidToHtmlElementId(Guid? guid, string prefix = "")
	{
		if (guid == null) return null;
		return $"{conversionPrefix}{(String.IsNullOrWhiteSpace(prefix) ? "" : $"{prefix}-")}{guid}";
	}
	public static Guid? ConvertHtmlElementIdToGuid(string htmlId)
	{
		if (String.IsNullOrWhiteSpace(htmlId)) return null;
		var match = htmlId.Trim().ToLowerInvariant();
		if (!match.StartsWith(conversionPrefix)) return null;

		match = match.Replace(conversionPrefix, "");

		if (Guid.TryParse(match, out Guid id)) return id;

		return null;
	}


	#endregion

	#region << User >>
	public static string GetUserInitials(TucUser user)
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

	#endregion

	#region << int >>
	public static int CalcSkip(int page, int pageSize) => (page - 1) * pageSize;
	#endregion

	#region << Icon >>
	public static List<string> GetSpaceIconLibrary()
	{
		if (_allIcons is not null) return _allIcons;

		var result = new List<string>();
		foreach (var item in IconsExtensions.AllIcons)
		{
			if (item.Size == TfConstants.IconSize
			&& item.Variant == TfConstants.IconVariant)
				result.Add(item.Name);
		}
		_allIcons = result.ToList();
		return _allIcons;
	}
	public static Icon ConvertFileNameToIcon(string fileName)
	{
		Icon result = TfConstants.GetIcon("Document");
		var extension = Path.GetExtension(fileName);

		switch (extension)
		{
			case ".txt":
				return TfConstants.GetIcon("DocumentText");
			case ".pdf":
				return TfConstants.GetIcon("DocumentPdf");
			case ".doc":
			case ".docx":
				return TfConstants.GetIcon("DocumentText");
			case ".xls":
			case ".xlsx":
				return TfConstants.GetIcon("DocumentTable");
			case ".ppt":
			case ".pptx":
				return TfConstants.GetIcon("DocumentData");
			case ".gif":
			case ".jpg":
			case ".jpeg":
			case ".png":
			case ".bmp":
			case ".tif":
				return TfConstants.GetIcon("Image");
			case ".zip":
			case ".zipx":
			case ".rar":
			case ".tar":
			case ".gz":
			case ".dmg":
			case ".iso":
				return TfConstants.GetIcon("FolderZip");
			case ".wav":
			case ".mp3":
			case ".fla":
			case ".flac":
			case ".ra":
			case ".rma":
			case ".aif":
			case ".aiff":
			case ".aa":
			case ".aac":
			case ".aax":
			case ".ac3":
			case ".au":
			case ".ogg":
			case ".avr":
			case ".3ga":
			case ".mid":
			case ".midi":
			case ".m4a":
			case ".mp4a":
			case ".amz":
			case ".mka":
			case ".asx":
			case ".pcm":
			case ".m3u":
			case ".wma":
			case ".xwma":
				return TfConstants.GetIcon("Speaker2");
			case ".avi":
			case ".mpg":
			case ".mp4":
			case ".mkv":
			case ".mov":
			case ".wmv":
			case ".vp6":
			case ".264":
			case ".vid":
			case ".rv":
			case ".webm":
			case ".swf":
			case ".h264":
			case ".flv":
			case ".mk3d":
			case ".gifv":
			case ".oggv":
			case ".3gp":
			case ".m4v":
			case ".movie":
			case ".divx":
				return TfConstants.GetIcon("Video");
			case ".c":
			case ".cpp":
			case ".css":
			case ".js":
			case ".py":
			case ".git":
			case ".cs":
			case ".cshtml":
			case ".xml":
			case ".html":
			case ".ini":
			case ".config":
			case ".json":
			case ".h":
			case ".htm":
			case ".xhtml":
			case ".php":
			case ".aspx":
				return TfConstants.GetIcon("Code");
			case ".exe":
			case ".jar":
			case ".dll":
			case ".bat":
			case ".pl":
			case ".scr":
			case ".msi":
			case ".app":
			case ".deb":
			case ".apk":
			case ".vb":
			case ".prg":
			case ".sh":
				return TfConstants.GetIcon("LauncherSettings");
			case ".com":
			case ".net":
			case ".org":
			case ".edu":
			case ".gov":
			case ".mil":
			case "/":
				return TfConstants.GetIcon("Link");
			default:
				return TfConstants.GetIcon("Document");
		}
	}
	#endregion

	#region << Dictionary >>
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

	#endregion

	#region << List >>
	public static TItem FindItemByMatch<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> matcher, Func<TItem, IEnumerable<TItem>> childGetter) where TItem : class
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
	#endregion


}
