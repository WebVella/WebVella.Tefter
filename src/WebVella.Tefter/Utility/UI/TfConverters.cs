namespace WebVella.Tefter.Utility;

public static partial class TfConverters
{
	private static string conversionPrefix = "tf-";
	private static List<string> _allIcons = null;

	#region << String >>
	public static string StringOverflow(string? input, int? charCount)
	{
		if (charCount is null
		|| String.IsNullOrWhiteSpace(input)
		|| input.Length <= charCount) return input ?? String.Empty;
		return input.Substring(0, charCount.Value) + "...";
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
	public static string GenerateDbNameFromText(this string text)
	{
		if (String.IsNullOrEmpty(text)) return text;
		text = text.Trim().ToLowerInvariant();
		text = text.Replace("№", "no");
		text = ConvertToLatin(text);
		Regex rgx = new Regex("[^a-zA-Z0-9]");
		text = rgx.Replace(text, "_");
		text = Regex.Replace(text, @"_+", "_");
		//remove starting or trailing _
		if (text.StartsWith("_"))
			text = text.Substring(1);
		if (text.EndsWith("_"))
			text = text.Substring(0, text.Length - 1);
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
	public static string ConvertGuidToHtmlElementId(Guid? guid = null, string prefix = "")
	{
		if (guid == null) guid = Guid.NewGuid();
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
	public static string GetUserInitials(TfUser user)
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

	public static List<TfFilterBase> ConvertQueryFilterToList(this List<TfFilterQuery>? items,
		List<TfSpaceViewColumn> viewColumns, List<TfDataProvider> providers, List<TfSharedColumn> sharedColumns)
	{
		var result = new List<TfFilterBase>();
		if (items is null || viewColumns is null || items.Count == 0 || viewColumns.Count == 0) return result;

		var typeDict = viewColumns.ToQueryNameTypeDictionary(dataProviders: providers, sharedColumns: sharedColumns);

		foreach (var item in items)
		{
			var filter = new TfFilterBase().FromQuery(item, viewColumns, typeDict);
			if (filter is null) continue;
			result.Add(filter);
		}

		return result;
	}

	public static List<TfSort> ConvertQuerySortToList(this List<TfSortQuery>? items, List<TfSpaceViewColumn> columns)
	{
		var result = new List<TfSort>();
		if (items is null || columns is null || items.Count == 0 || columns.Count == 0) return result;

		foreach (var item in items)
		{
			var sort = new TfSort().FromQuery(item, columns);
			result.Add(sort);
		}

		return result;
	}

	public static Dictionary<string, TfDatabaseColumnType> ToQueryNameTypeDictionary(this List<TfSpaceViewColumn> viewColumns,
			List<TfDataProvider> dataProviders, List<TfSharedColumn> sharedColumns)
	{
		Dictionary<string, TfDatabaseColumnType> typeDict = new();

		Dictionary<string, TfDatabaseColumnType> columnNameTypeDict = new();
		foreach (var dp in dataProviders)
		{
			foreach (var col in dp.Columns)
			{
				if (String.IsNullOrWhiteSpace(col.DbName)) continue;
				columnNameTypeDict[col.DbName] = col.DbType;
			}
		}

		foreach (var col in sharedColumns)
		{
			if (String.IsNullOrWhiteSpace(col.DbName)) continue;
			columnNameTypeDict[col.DbName] = col.DbType;
		}

		foreach (var column in viewColumns)
		{
			var columnName = column.GetColumnNameFromDataMapping();
			if (String.IsNullOrWhiteSpace(columnName)) continue;
			var nameArray = columnName.Split(".");
			if (nameArray.Length == 1)
			{
				//it is a local provider column
				if (!columnNameTypeDict.ContainsKey(columnName)) continue;
				typeDict[column.QueryName] = columnNameTypeDict[columnName];
			}
			else
			{
				if (!columnNameTypeDict.ContainsKey(nameArray[1])) continue;
				typeDict[column.QueryName] = columnNameTypeDict[nameArray[1]];
			}
		}


		return typeDict;
	}

	#endregion

	#region << Color >>
	public static void GenerateColorsFromVariables()
	{
		var colorJSON = @"
[
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-slate-50"",
    ""oklch_value"": ""oklch(0.984 0.003 247.858)"",
    ""closest_hex_value"": ""#f8fafc""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-slate-100"",
    ""oklch_value"": ""oklch(0.957 0.009 250.395)"",
    ""closest_hex_value"": ""#f1f5f9""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-slate-200"",
    ""oklch_value"": ""oklch(0.916 0.016 252.883)"",
    ""closest_hex_value"": ""#e2e8f0""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-slate-300"",
    ""oklch_value"": ""oklch(0.846 0.024 254.912)"",
    ""closest_hex_value"": ""#cbd5e1""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-slate-400"",
    ""oklch_value"": ""oklch(0.729 0.035 256.401)"",
    ""closest_hex_value"": ""#94a3b8""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-slate-500"",
    ""oklch_value"": ""oklch(0.554 0.046 257.417)"",
    ""closest_hex_value"": ""#64748b""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-slate-600"",
    ""oklch_value"": ""oklch(0.463 0.047 259.948)"",
    ""closest_hex_value"": ""#475569""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-slate-700"",
    ""oklch_value"": ""oklch(0.384 0.046 262.835)"",
    ""closest_hex_value"": ""#334155""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-slate-800"",
    ""oklch_value"": ""oklch(0.312 0.043 266.027)"",
    ""closest_hex_value"": ""#1e293b""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-slate-900"",
    ""oklch_value"": ""oklch(0.245 0.038 269.467)"",
    ""closest_hex_value"": ""#0f172a""
  },
  {
    ""color_type"": ""slate"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-slate-950"",
    ""oklch_value"": ""oklch(0.134 0.035 272.96)"",
    ""closest_hex_value"": ""#020617""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-gray-50"",
    ""oklch_value"": ""oklch(0.984 0.003 264.66)"",
    ""closest_hex_value"": ""#fafafa""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-gray-100"",
    ""oklch_value"": ""oklch(0.957 0.009 264.66)"",
    ""closest_hex_value"": ""#f4f4f5""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-gray-200"",
    ""oklch_value"": ""oklch(0.916 0.017 264.66)"",
    ""closest_hex_value"": ""#e4e4e7""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-gray-300"",
    ""oklch_value"": ""oklch(0.846 0.027 264.66)"",
    ""closest_hex_value"": ""#d4d4d8""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-gray-400"",
    ""oklch_value"": ""oklch(0.729 0.039 264.66)"",
    ""closest_hex_value"": ""#a1a1aa""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-gray-500"",
    ""oklch_value"": ""oklch(0.554 0.047 264.66)"",
    ""closest_hex_value"": ""#71717a""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-gray-600"",
    ""oklch_value"": ""oklch(0.463 0.047 264.66)"",
    ""closest_hex_value"": ""#52525b""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-gray-700"",
    ""oklch_value"": ""oklch(0.384 0.046 264.66)"",
    ""closest_hex_value"": ""#3f3f46""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-gray-800"",
    ""oklch_value"": ""oklch(0.312 0.043 264.66)"",
    ""closest_hex_value"": ""#27272a""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-gray-900"",
    ""oklch_value"": ""oklch(0.245 0.038 264.66)"",
    ""closest_hex_value"": ""#18181b""
  },
  {
    ""color_type"": ""gray"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-gray-950"",
    ""oklch_value"": ""oklch(0.134 0.036 264.66)"",
    ""closest_hex_value"": ""#09090b""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-zinc-50"",
    ""oklch_value"": ""oklch(0.984 0.003 264.66)"",
    ""closest_hex_value"": ""#fafafa""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-zinc-100"",
    ""oklch_value"": ""oklch(0.957 0.009 264.66)"",
    ""closest_hex_value"": ""#f4f4f5""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-zinc-200"",
    ""oklch_value"": ""oklch(0.916 0.016 264.66)"",
    ""closest_hex_value"": ""#e4e4e7""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-zinc-300"",
    ""oklch_value"": ""oklch(0.846 0.024 264.66)"",
    ""closest_hex_value"": ""#d4d4d8""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-zinc-400"",
    ""oklch_value"": ""oklch(0.729 0.035 264.66)"",
    ""closest_hex_value"": ""#a1a1aa""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-zinc-500"",
    ""oklch_value"": ""oklch(0.554 0.047 264.66)"",
    ""closest_hex_value"": ""#71717a""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-zinc-600"",
    ""oklch_value"": ""oklch(0.463 0.047 264.66)"",
    ""closest_hex_value"": ""#52525b""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-zinc-700"",
    ""oklch_value"": ""oklch(0.384 0.046 264.66)"",
    ""closest_hex_value"": ""#3f3f46""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-zinc-800"",
    ""oklch_value"": ""oklch(0.312 0.043 264.66)"",
    ""closest_hex_value"": ""#27272a""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-zinc-900"",
    ""oklch_value"": ""oklch(0.245 0.038 264.66)"",
    ""closest_hex_value"": ""#18181b""
  },
  {
    ""color_type"": ""zinc"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-zinc-950"",
    ""oklch_value"": ""oklch(0.134 0.036 264.66)"",
    ""closest_hex_value"": ""#09090b""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-neutral-50"",
    ""oklch_value"": ""oklch(0.984 0.003 264.66)"",
    ""closest_hex_value"": ""#fafafa""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-neutral-100"",
    ""oklch_value"": ""oklch(0.957 0.009 264.66)"",
    ""closest_hex_value"": ""#f4f4f5""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-neutral-200"",
    ""oklch_value"": ""oklch(0.916 0.016 264.66)"",
    ""closest_hex_value"": ""#e4e4e7""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-neutral-300"",
    ""oklch_value"": ""oklch(0.846 0.024 264.66)"",
    ""closest_hex_value"": ""#d4d4d8""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-neutral-400"",
    ""oklch_value"": ""oklch(0.729 0.035 264.66)"",
    ""closest_hex_value"": ""#a1a1aa""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-neutral-500"",
    ""oklch_value"": ""oklch(0.554 0.047 264.66)"",
    ""closest_hex_value"": ""#71717a""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-neutral-600"",
    ""oklch_value"": ""oklch(0.463 0.047 264.66)"",
    ""closest_hex_value"": ""#52525b""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-neutral-700"",
    ""oklch_value"": ""oklch(0.384 0.046 264.66)"",
    ""closest_hex_value"": ""#3f3f46""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-neutral-800"",
    ""oklch_value"": ""oklch(0.312 0.043 264.66)"",
    ""closest_hex_value"": ""#27272a""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-neutral-900"",
    ""oklch_value"": ""oklch(0.245 0.038 264.66)"",
    ""closest_hex_value"": ""#18181b""
  },
  {
    ""color_type"": ""neutral"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-neutral-950"",
    ""oklch_value"": ""oklch(0.134 0.036 264.66)"",
    ""closest_hex_value"": ""#09090b""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-stone-50"",
    ""oklch_value"": ""oklch(0.984 0.003 264.66)"",
    ""closest_hex_value"": ""#fafaf9""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-stone-100"",
    ""oklch_value"": ""oklch(0.957 0.009 264.66)"",
    ""closest_hex_value"": ""#f5f5f4""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-stone-200"",
    ""oklch_value"": ""oklch(0.916 0.016 264.66)"",
    ""closest_hex_value"": ""#e7e5e4""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-stone-300"",
    ""oklch_value"": ""oklch(0.846 0.024 264.66)"",
    ""closest_hex_value"": ""#d6d3d1""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-stone-400"",
    ""oklch_value"": ""oklch(0.729 0.035 264.66)"",
    ""closest_hex_value"": ""#a8a29e""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-stone-500"",
    ""oklch_value"": ""oklch(0.554 0.047 264.66)"",
    ""closest_hex_value"": ""#78716c""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-stone-600"",
    ""oklch_value"": ""oklch(0.463 0.047 264.66)"",
    ""closest_hex_value"": ""#57534e""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-stone-700"",
    ""oklch_value"": ""oklch(0.384 0.046 264.66)"",
    ""closest_hex_value"": ""#44403c""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-stone-800"",
    ""oklch_value"": ""oklch(0.312 0.043 264.66)"",
    ""closest_hex_value"": ""#292524""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-stone-900"",
    ""oklch_value"": ""oklch(0.245 0.038 264.66)"",
    ""closest_hex_value"": ""#1c1917""
  },
  {
    ""color_type"": ""stone"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-stone-950"",
    ""oklch_value"": ""oklch(0.134 0.036 264.66)"",
    ""closest_hex_value"": ""#0c0a09""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-red-50"",
    ""oklch_value"": ""oklch(0.971 0.013 17.38)"",
    ""closest_hex_value"": ""#fef2f2""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-red-100"",
    ""oklch_value"": ""oklch(0.932 0.042 21.033)"",
    ""closest_hex_value"": ""#fee2e2""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-red-200"",
    ""oklch_value"": ""oklch(0.865 0.089 23.367)"",
    ""closest_hex_value"": ""#fecaca""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-red-300"",
    ""oklch_value"": ""oklch(0.771 0.146 24.62)"",
    ""closest_hex_value"": ""#fca5a5""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-red-400"",
    ""oklch_value"": ""oklch(0.697 0.198 25.131)"",
    ""closest_hex_value"": ""#f87171""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-red-500"",
    ""oklch_value"": ""oklch(0.637 0.237 25.331)"",
    ""closest_hex_value"": ""#ef4444""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-red-600"",
    ""oklch_value"": ""oklch(0.551 0.237 25.689)"",
    ""closest_hex_value"": ""#dc2626""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-red-700"",
    ""oklch_value"": ""oklch(0.446 0.196 26.046)"",
    ""closest_hex_value"": ""#b91c1c""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-red-800"",
    ""oklch_value"": ""oklch(0.375 0.155 26.059)"",
    ""closest_hex_value"": ""#991b1b""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-red-900"",
    ""oklch_value"": ""oklch(0.314 0.117 26.043)"",
    ""closest_hex_value"": ""#7f1d1d""
  },
  {
    ""color_type"": ""red"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-red-950"",
    ""oklch_value"": ""oklch(0.258 0.092 26.042)"",
    ""closest_hex_value"": ""#450a0a""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-orange-50"",
    ""oklch_value"": ""oklch(0.975 0.016 57.073)"",
    ""closest_hex_value"": ""#fff7ed""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-orange-100"",
    ""oklch_value"": ""oklch(0.945 0.046 59.907)"",
    ""closest_hex_value"": ""#ffedd5""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-orange-200"",
    ""oklch_value"": ""oklch(0.887 0.091 62.776)"",
    ""closest_hex_value"": ""#fed7aa""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-orange-300"",
    ""oklch_value"": ""oklch(0.799 0.144 65.656)"",
    ""closest_hex_value"": ""#fdba74""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-orange-400"",
    ""oklch_value"": ""oklch(0.718 0.198 68.327)"",
    ""closest_hex_value"": ""#fb923c""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-orange-500"",
    ""oklch_value"": ""oklch(0.655 0.245 70.329)"",
    ""closest_hex_value"": ""#f97316""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-orange-600"",
    ""oklch_value"": ""oklch(0.575 0.254 71.393)"",
    ""closest_hex_value"": ""#ea580c""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-orange-700"",
    ""oklch_value"": ""oklch(0.473 0.222 72.396)"",
    ""closest_hex_value"": ""#c2410c""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-orange-800"",
    ""oklch_value"": ""oklch(0.395 0.181 73.181)"",
    ""closest_hex_value"": ""#9a3412""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-orange-900"",
    ""oklch_value"": ""oklch(0.328 0.141 73.91)"",
    ""closest_hex_value"": ""#7c2d12""
  },
  {
    ""color_type"": ""orange"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-orange-950"",
    ""oklch_value"": ""oklch(0.267 0.103 74.631)"",
    ""closest_hex_value"": ""#431407""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-amber-50"",
    ""oklch_value"": ""oklch(0.985 0.021 82.355)"",
    ""closest_hex_value"": ""#fffbeb""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-amber-100"",
    ""oklch_value"": ""oklch(0.963 0.052 83.25)"",
    ""closest_hex_value"": ""#fef3c7""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-amber-200"",
    ""oklch_value"": ""oklch(0.923 0.094 84.148)"",
    ""closest_hex_value"": ""#fde68a""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-amber-300"",
    ""oklch_value"": ""oklch(0.859 0.141 85.039)"",
    ""closest_hex_value"": ""#fcd34d""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-amber-400"",
    ""oklch_value"": ""oklch(0.771 0.191 85.834)"",
    ""closest_hex_value"": ""#fbbf24""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-amber-500"",
    ""oklch_value"": ""oklch(0.672 0.231 86.419)"",
    ""closest_hex_value"": ""#f59e0b""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-amber-600"",
    ""oklch_value"": ""oklch(0.612 0.236 86.619)"",
    ""closest_hex_value"": ""#d97706""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-amber-700"",
    ""oklch_value"": ""oklch(0.509 0.205 86.732)"",
    ""closest_hex_value"": ""#b45309""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-amber-800"",
    ""oklch_value"": ""oklch(0.427 0.165 86.829)"",
    ""closest_hex_value"": ""#92400e""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-amber-900"",
    ""oklch_value"": ""oklch(0.354 0.128 86.924)"",
    ""closest_hex_value"": ""#78350f""
  },
  {
    ""color_type"": ""amber"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-amber-950"",
    ""oklch_value"": ""oklch(0.286 0.088 87.054)"",
    ""closest_hex_value"": ""#451a03""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-yellow-50"",
    ""oklch_value"": ""oklch(0.987 0.026 102.212)"",
    ""closest_hex_value"": ""#fefce8""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-yellow-100"",
    ""oklch_value"": ""oklch(0.969 0.063 97.942)"",
    ""closest_hex_value"": ""#fef9c3""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-yellow-200"",
    ""oklch_value"": ""oklch(0.932 0.106 93.978)"",
    ""closest_hex_value"": ""#fef08a""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-yellow-300"",
    ""oklch_value"": ""oklch(0.873 0.147 90.725)"",
    ""closest_hex_value"": ""#fde047""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-yellow-400"",
    ""oklch_value"": ""oklch(0.812 0.176 88.358)"",
    ""closest_hex_value"": ""#facc15""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-yellow-500"",
    ""oklch_value"": ""oklch(0.795 0.184 86.047)"",
    ""closest_hex_value"": ""#eab308""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-yellow-600"",
    ""oklch_value"": ""oklch(0.686 0.187 83.218)"",
    ""closest_hex_value"": ""#ca8a04""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-yellow-700"",
    ""oklch_value"": ""oklch(0.551 0.154 78.411)"",
    ""closest_hex_value"": ""#a16207""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-yellow-800"",
    ""oklch_value"": ""oklch(0.457 0.117 72.843)"",
    ""closest_hex_value"": ""#854d0e""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-yellow-900"",
    ""oklch_value"": ""oklch(0.378 0.084 66.273)"",
    ""closest_hex_value"": ""#713f12""
  },
  {
    ""color_type"": ""yellow"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-yellow-950"",
    ""oklch_value"": ""oklch(0.286 0.066 53.813)"",
    ""closest_hex_value"": ""#422006""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-lime-50"",
    ""oklch_value"": ""oklch(0.985 0.013 121.785)"",
    ""closest_hex_value"": ""#f7fee7""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-lime-100"",
    ""oklch_value"": ""oklch(0.967 0.031 120.306)"",
    ""closest_hex_value"": ""#ecfccb""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-lime-200"",
    ""oklch_value"": ""oklch(0.932 0.057 118.827)"",
    ""closest_hex_value"": ""#d9f99d""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-lime-300"",
    ""oklch_value"": ""oklch(0.877 0.089 117.387)"",
    ""closest_hex_value"": ""#bef264""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-lime-400"",
    ""oklch_value"": ""oklch(0.793 0.126 116.035)"",
    ""closest_hex_value"": ""#a3e635""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-lime-500"",
    ""oklch_value"": ""oklch(0.686 0.171 114.718)"",
    ""closest_hex_value"": ""#84cc16""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-lime-600"",
    ""oklch_value"": ""oklch(0.603 0.177 113.886)"",
    ""closest_hex_value"": ""#65a30d""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-lime-700"",
    ""oklch_value"": ""oklch(0.485 0.157 112.433)"",
    ""closest_hex_value"": ""#4f7c06""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-lime-800"",
    ""oklch_value"": ""oklch(0.407 0.126 109.914)"",
    ""closest_hex_value"": ""#4a491d""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-lime-900"",
    ""oklch_value"": ""oklch(0.342 0.096 104.996)"",
    ""closest_hex_value"": ""#3f3f20""
  },
  {
    ""color_type"": ""lime"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-lime-950"",
    ""oklch_value"": ""oklch(0.274 0.063 94.618)"",
    ""closest_hex_value"": ""#1f220f""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-green-50"",
    ""oklch_value"": ""oklch(0.981 0.015 142.062)"",
    ""closest_hex_value"": ""#f0fdf4""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-green-100"",
    ""oklch_value"": ""oklch(0.95 0.038 144.173)"",
    ""closest_hex_value"": ""#dcfce7""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-green-200"",
    ""oklch_value"": ""oklch(0.901 0.071 146.284)"",
    ""closest_hex_value"": ""#bbf7d0""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-green-300"",
    ""oklch_value"": ""oklch(0.829 0.108 148.423)"",
    ""closest_hex_value"": ""#86efad""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-green-400"",
    ""oklch_value"": ""oklch(0.741 0.148 150.563)"",
    ""closest_hex_value"": ""#4ade80""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-green-500"",
    ""oklch_value"": ""oklch(0.643 0.187 152.748)"",
    ""closest_hex_value"": ""#22c55e""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-green-600"",
    ""oklch_value"": ""oklch(0.551 0.186 154.341)"",
    ""closest_hex_value"": ""#16a34a""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-green-700"",
    ""oklch_value"": ""oklch(0.448 0.158 155.938)"",
    ""closest_hex_value"": ""#15803d""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-green-800"",
    ""oklch_value"": ""oklch(0.374 0.126 157.19)"",
    ""closest_hex_value"": ""#166534""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-green-900"",
    ""oklch_value"": ""oklch(0.312 0.096 158.334)"",
    ""closest_hex_value"": ""#14532d""
  },
  {
    ""color_type"": ""green"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-green-950"",
    ""oklch_value"": ""oklch(0.258 0.063 159.213)"",
    ""closest_hex_value"": ""#052e16""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-emerald-50"",
    ""oklch_value"": ""oklch(0.979 0.021 166.113)"",
    ""closest_hex_value"": ""#ecfdf5""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-emerald-100"",
    ""oklch_value"": ""oklch(0.947 0.057 165.746)"",
    ""closest_hex_value"": ""#d1fae5""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-emerald-200"",
    ""oklch_value"": ""oklch(0.898 0.096 165.378)"",
    ""closest_hex_value"": ""#a7f3d0""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-emerald-300"",
    ""oklch_value"": ""oklch(0.829 0.137 165.008)"",
    ""closest_hex_value"": ""#6ee7b7""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-emerald-400"",
    ""oklch_value"": ""oklch(0.743 0.176 164.636)"",
    ""closest_hex_value"": ""#34d399""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-emerald-500"",
    ""oklch_value"": ""oklch(0.696 0.17 162.48)"",
    ""closest_hex_value"": ""#10b981""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-emerald-600"",
    ""oklch_value"": ""oklch(0.589 0.17 161.947)"",
    ""closest_hex_value"": ""#059669""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-emerald-700"",
    ""oklch_value"": ""oklch(0.487 0.151 161.414)"",
    ""closest_hex_value"": ""#047857""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-emerald-800"",
    ""oklch_value"": ""oklch(0.412 0.125 160.884)"",
    ""closest_hex_value"": ""#065f46""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-emerald-900"",
    ""oklch_value"": ""oklch(0.347 0.098 160.359)"",
    ""closest_hex_value"": ""#064e3b""
  },
  {
    ""color_type"": ""emerald"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-emerald-950"",
    ""oklch_value"": ""oklch(0.262 0.051 172.552)"",
    ""closest_hex_value"": ""#022c22""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-teal-50"",
    ""oklch_value"": ""oklch(0.973 0.026 195.457)"",
    ""closest_hex_value"": ""#f0fdfa""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-teal-100"",
    ""oklch_value"": ""oklch(0.938 0.063 194.27)"",
    ""closest_hex_value"": ""#ccfbf1""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-teal-200"",
    ""oklch_value"": ""oklch(0.885 0.106 193.083)"",
    ""closest_hex_value"": ""#99f6e4""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-teal-300"",
    ""oklch_value"": ""oklch(0.814 0.147 191.884)"",
    ""closest_hex_value"": ""#5eead4""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-teal-400"",
    ""oklch_value"": ""oklch(0.729 0.176 190.672)"",
    ""closest_hex_value"": ""#2dd4bf""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-teal-500"",
    ""oklch_value"": ""oklch(0.613 0.184 188.751)"",
    ""closest_hex_value"": ""#14b8a6""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-teal-600"",
    ""oklch_value"": ""oklch(0.518 0.173 187.818)"",
    ""closest_hex_value"": ""#0d9488""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-teal-700"",
    ""oklch_value"": ""oklch(0.426 0.148 186.884)"",
    ""closest_hex_value"": ""#0f766e""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-teal-800"",
    ""oklch_value"": ""oklch(0.354 0.118 186.115)"",
    ""closest_hex_value"": ""#115e59""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-teal-900"",
    ""oklch_value"": ""oklch(0.293 0.091 185.358)"",
    ""closest_hex_value"": ""#134e4a""
  },
  {
    ""color_type"": ""teal"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-teal-950"",
    ""oklch_value"": ""oklch(0.245 0.063 183.82)"",
    ""closest_hex_value"": ""#042f2e""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-cyan-50"",
    ""oklch_value"": ""oklch(0.975 0.034 216.517)"",
    ""closest_hex_value"": ""#f0faff""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-cyan-100"",
    ""oklch_value"": ""oklch(0.94 0.081 216.517)"",
    ""closest_hex_value"": ""#cffafe""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-cyan-200"",
    ""oklch_value"": ""oklch(0.884 0.134 216.517)"",
    ""closest_hex_value"": ""#a5f3fc""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-cyan-300"",
    ""oklch_value"": ""oklch(0.806 0.191 216.517)"",
    ""closest_hex_value"": ""#67e8f9""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-cyan-400"",
    ""oklch_value"": ""oklch(0.717 0.248 216.517)"",
    ""closest_hex_value"": ""#22d3ee""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-cyan-500"",
    ""oklch_value"": ""oklch(0.609 0.297 216.517)"",
    ""closest_hex_value"": ""#06b6d4""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-cyan-600"",
    ""oklch_value"": ""oklch(0.509 0.288 216.517)"",
    ""closest_hex_value"": ""#0891b2""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-cyan-700"",
    ""oklch_value"": ""oklch(0.418 0.252 216.517)"",
    ""closest_hex_value"": ""#0e7490""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-cyan-800"",
    ""oklch_value"": ""oklch(0.347 0.208 216.517)"",
    ""closest_hex_value"": ""#155e75""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-cyan-900"",
    ""oklch_value"": ""oklch(0.288 0.165 216.517)"",
    ""closest_hex_value"": ""#164e63""
  },
  {
    ""color_type"": ""cyan"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-cyan-950"",
    ""oklch_value"": ""oklch(0.243 0.119 216.517)"",
    ""closest_hex_value"": ""#083344""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-sky-50"",
    ""oklch_value"": ""oklch(0.963 0.052 238.163)"",
    ""closest_hex_value"": ""#f0f9ff""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-sky-100"",
    ""oklch_value"": ""oklch(0.92 0.116 238.687)"",
    ""closest_hex_value"": ""#e0f2fe""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-sky-200"",
    ""oklch_value"": ""oklch(0.85 0.192 239.231)"",
    ""closest_hex_value"": ""#bae6fd""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-sky-300"",
    ""oklch_value"": ""oklch(0.749 0.273 239.814)"",
    ""closest_hex_value"": ""#7dd3fc""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-sky-400"",
    ""oklch_value"": ""oklch(0.635 0.353 240.447)"",
    ""closest_hex_value"": ""#38bdf8""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-sky-500"",
    ""oklch_value"": ""oklch(0.536 0.384 240.243)"",
    ""closest_hex_value"": ""#0ea5e9""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-sky-600"",
    ""oklch_value"": ""oklch(0.449 0.375 239.825)"",
    ""closest_hex_value"": ""#0284c7""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-sky-700"",
    ""oklch_value"": ""oklch(0.368 0.334 239.387)"",
    ""closest_hex_value"": ""#0369a1""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-sky-800"",
    ""oklch_value"": ""oklch(0.301 0.279 238.922)"",
    ""closest_hex_value"": ""#075985""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-sky-900"",
    ""oklch_value"": ""oklch(0.244 0.224 238.423)"",
    ""closest_hex_value"": ""#0c4a6e""
  },
  {
    ""color_type"": ""sky"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-sky-950"",
    ""oklch_value"": ""oklch(0.211 0.165 241.144)"",
    ""closest_hex_value"": ""#082f49""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-blue-50"",
    ""oklch_value"": ""oklch(0.956 0.038 250.222)"",
    ""closest_hex_value"": ""#eff6ff""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-blue-100"",
    ""oklch_value"": ""oklch(0.908 0.091 251.493)"",
    ""closest_hex_value"": ""#dbeafe""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-blue-200"",
    ""oklch_value"": ""oklch(0.835 0.151 252.812)"",
    ""closest_hex_value"": ""#bfdbfe""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-blue-300"",
    ""oklch_value"": ""oklch(0.729 0.221 254.195)"",
    ""closest_hex_value"": ""#93c5fd""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-blue-400"",
    ""oklch_value"": ""oklch(0.617 0.291 255.658)"",
    ""closest_hex_value"": ""#60a5fa""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-blue-500"",
    ""oklch_value"": ""oklch(0.573 0.151 262.115)"",
    ""closest_hex_value"": ""#3b82f6""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-blue-600"",
    ""oklch_value"": ""oklch(0.485 0.134 266.368)"",
    ""closest_hex_value"": ""#2563eb""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-blue-700"",
    ""oklch_value"": ""oklch(0.395 0.113 270.612)"",
    ""closest_hex_value"": ""#1d4ed8""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-blue-800"",
    ""oklch_value"": ""oklch(0.322 0.093 274.79)"",
    ""closest_hex_value"": ""#1e40af""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-blue-900"",
    ""oklch_value"": ""oklch(0.264 0.076 278.892)"",
    ""closest_hex_value"": ""#1e3a8a""
  },
  {
    ""color_type"": ""blue"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-blue-950"",
    ""oklch_value"": ""oklch(0.262 0.076 270.366)"",
    ""closest_hex_value"": ""#172554""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-indigo-50"",
    ""oklch_value"": ""oklch(0.967 0.027 289.445)"",
    ""closest_hex_value"": ""#eef2ff""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-indigo-100"",
    ""oklch_value"": ""oklch(0.932 0.065 291.564)"",
    ""closest_hex_value"": ""#e0e7ff""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-indigo-200"",
    ""oklch_value"": ""oklch(0.868 0.117 293.682)"",
    ""closest_hex_value"": ""#c7d2fe""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-indigo-300"",
    ""oklch_value"": ""oklch(0.778 0.177 295.795)"",
    ""closest_hex_value"": ""#a5b4fc""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-indigo-400"",
    ""oklch_value"": ""oklch(0.677 0.244 297.904)"",
    ""closest_hex_value"": ""#818cf8""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-indigo-500"",
    ""oklch_value"": ""oklch(0.573 0.252 298.374)"",
    ""closest_hex_value"": ""#6366f1""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-indigo-600"",
    ""oklch_value"": ""oklch(0.485 0.244 298.835)"",
    ""closest_hex_value"": ""#4f46e5""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-indigo-700"",
    ""oklch_value"": ""oklch(0.395 0.218 299.284)"",
    ""closest_hex_value"": ""#4338ca""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-indigo-800"",
    ""oklch_value"": ""oklch(0.322 0.183 299.719)"",
    ""closest_hex_value"": ""#3730a3""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-indigo-900"",
    ""oklch_value"": ""oklch(0.264 0.149 300.14)"",
    ""closest_hex_value"": ""#312e81""
  },
  {
    ""color_type"": ""indigo"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-indigo-950"",
    ""oklch_value"": ""oklch(0.211 0.111 300.569)"",
    ""closest_hex_value"": ""#1e1b4b""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-violet-50"",
    ""oklch_value"": ""oklch(0.975 0.021 303.498)"",
    ""closest_hex_value"": ""#f5f3ff""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-violet-100"",
    ""oklch_value"": ""oklch(0.941 0.051 305.823)"",
    ""closest_hex_value"": ""#ede9fe""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-violet-200"",
    ""oklch_value"": ""oklch(0.88 0.098 308.148)"",
    ""closest_hex_value"": ""#ddd6fe""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-violet-300"",
    ""oklch_value"": ""oklch(0.793 0.155 310.468)"",
    ""closest_hex_value"": ""#c4b5fd""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-violet-400"",
    ""oklch_value"": ""oklch(0.692 0.222 312.784)"",
    ""closest_hex_value"": ""#a78bfa""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-violet-500"",
    ""oklch_value"": ""oklch(0.573 0.248 314.544)"",
    ""closest_hex_value"": ""#8b5cf6""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-violet-600"",
    ""oklch_value"": ""oklch(0.485 0.24 315.487)"",
    ""closest_hex_value"": ""#7c3aed""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-violet-700"",
    ""oklch_value"": ""oklch(0.395 0.213 316.427)"",
    ""closest_hex_value"": ""#6d28d9""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-violet-800"",
    ""oklch_value"": ""oklch(0.322 0.178 317.361)"",
    ""closest_hex_value"": ""#5b21b6""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-violet-900"",
    ""oklch_value"": ""oklch(0.264 0.144 318.289)"",
    ""closest_hex_value"": ""#4c1d95""
  },
  {
    ""color_type"": ""violet"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-violet-950"",
    ""oklch_value"": ""oklch(0.211 0.106 319.227)"",
    ""closest_hex_value"": ""#2e105e""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-purple-50"",
    ""oklch_value"": ""oklch(0.975 0.016 308.232)"",
    ""closest_hex_value"": ""#faf5ff""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-purple-100"",
    ""oklch_value"": ""oklch(0.941 0.04 311.166)"",
    ""closest_hex_value"": ""#f3e8ff""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-purple-200"",
    ""oklch_value"": ""oklch(0.88 0.076 314.098)"",
    ""closest_hex_value"": ""#e9d5ff""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-purple-300"",
    ""oklch_value"": ""oklch(0.793 0.118 317.027)"",
    ""closest_hex_value"": ""#d8b4fe""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-purple-400"",
    ""oklch_value"": ""oklch(0.692 0.158 319.952)"",
    ""closest_hex_value"": ""#c084fc""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-purple-500"",
    ""oklch_value"": ""oklch(0.551 0.141 316.533)"",
    ""closest_hex_value"": ""#a855f7""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-purple-600"",
    ""oklch_value"": ""oklch(0.463 0.134 316.34)"",
    ""closest_hex_value"": ""#9333ea""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-purple-700"",
    ""oklch_value"": ""oklch(0.372 0.119 316.147)"",
    ""closest_hex_value"": ""#7e22ce""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-purple-800"",
    ""oklch_value"": ""oklch(0.302 0.101 315.954)"",
    ""closest_hex_value"": ""#6b21a8""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-purple-900"",
    ""oklch_value"": ""oklch(0.245 0.083 315.761)"",
    ""closest_hex_value"": ""#581c87""
  },
  {
    ""color_type"": ""purple"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-purple-950"",
    ""oklch_value"": ""oklch(0.266 0.071 315.688)"",
    ""closest_hex_value"": ""#3b0764""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-fuchsia-50"",
    ""oklch_value"": ""oklch(0.971 0.038 328.799)"",
    ""closest_hex_value"": ""#faf3ff""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-fuchsia-100"",
    ""oklch_value"": ""oklch(0.933 0.082 331.439)"",
    ""closest_hex_value"": ""#f5d0fe""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-fuchsia-200"",
    ""oklch_value"": ""oklch(0.868 0.143 334.08)"",
    ""closest_hex_value"": ""#f0abfc""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-fuchsia-300"",
    ""oklch_value"": ""oklch(0.778 0.218 336.72)"",
    ""closest_hex_value"": ""#e879f9""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-fuchsia-400"",
    ""oklch_value"": ""oklch(0.677 0.301 339.359)"",
    ""closest_hex_value"": ""#e049fa""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-fuchsia-500"",
    ""oklch_value"": ""oklch(0.573 0.304 340.52)"",
    ""closest_hex_value"": ""#d946ef""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-fuchsia-600"",
    ""oklch_value"": ""oklch(0.485 0.29 341.68)"",
    ""closest_hex_value"": ""#c026d3""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-fuchsia-700"",
    ""oklch_value"": ""oklch(0.395 0.256 342.84)"",
    ""closest_hex_value"": ""#a21caf""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-fuchsia-800"",
    ""oklch_value"": ""oklch(0.322 0.215 344.0)"",
    ""closest_hex_value"": ""#86198f""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-fuchsia-900"",
    ""oklch_value"": ""oklch(0.264 0.174 345.158)"",
    ""closest_hex_value"": ""#701a75""
  },
  {
    ""color_type"": ""fuchsia"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-fuchsia-950"",
    ""oklch_value"": ""oklch(0.211 0.131 346.315)"",
    ""closest_hex_value"": ""#4a044e""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-pink-50"",
    ""oklch_value"": ""oklch(0.975 0.026 350.231)"",
    ""closest_hex_value"": ""#fdf2f8""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-pink-100"",
    ""oklch_value"": ""oklch(0.941 0.066 351.487)"",
    ""closest_hex_value"": ""#fce7f3""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-pink-200"",
    ""oklch_value"": ""oklch(0.88 0.123 352.743)"",
    ""closest_hex_value"": ""#fbcfe8""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-pink-300"",
    ""oklch_value"": ""oklch(0.793 0.191 353.999)"",
    ""closest_hex_value"": ""#f9a8d4""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-pink-400"",
    ""oklch_value"": ""oklch(0.692 0.264 355.253)"",
    ""closest_hex_value"": ""#f472b6""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-pink-500"",
    ""oklch_value"": ""oklch(0.573 0.283 356.508)"",
    ""closest_hex_value"": ""#ec4899""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-pink-600"",
    ""oklch_value"": ""oklch(0.485 0.269 357.172)"",
    ""closest_hex_value"": ""#db2777""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-pink-700"",
    ""oklch_value"": ""oklch(0.395 0.233 357.836)"",
    ""closest_hex_value"": ""#be185e""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-pink-800"",
    ""oklch_value"": ""oklch(0.322 0.192 358.5)"",
    ""closest_hex_value"": ""#9d174d""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-pink-900"",
    ""oklch_value"": ""oklch(0.264 0.151 359.164)"",
    ""closest_hex_value"": ""#831843""
  },
  {
    ""color_type"": ""pink"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-pink-950"",
    ""oklch_value"": ""oklch(0.211 0.111 359.827)"",
    ""closest_hex_value"": ""#4a0429""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""50"",
    ""variable_name"": ""--color-rose-50"",
    ""oklch_value"": ""oklch(0.971 0.019 14.88)"",
    ""closest_hex_value"": ""#fff1f2""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""100"",
    ""variable_name"": ""--color-rose-100"",
    ""oklch_value"": ""oklch(0.932 0.057 18.006)"",
    ""closest_hex_value"": ""#ffe4e6""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""200"",
    ""variable_name"": ""--color-rose-200"",
    ""oklch_value"": ""oklch(0.865 0.113 21.132)"",
    ""closest_hex_value"": ""#fecdd3""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""300"",
    ""variable_name"": ""--color-rose-300"",
    ""oklch_value"": ""oklch(0.771 0.183 24.257)"",
    ""closest_hex_value"": ""#fda4af""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""400"",
    ""variable_name"": ""--color-rose-400"",
    ""oklch_value"": ""oklch(0.697 0.252 27.38)"",
    ""closest_hex_value"": ""#fb7185""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""500"",
    ""variable_name"": ""--color-rose-500"",
    ""oklch_value"": ""oklch(0.637 0.297 30.504)"",
    ""closest_hex_value"": ""#f43f5e""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""600"",
    ""variable_name"": ""--color-rose-600"",
    ""oklch_value"": ""oklch(0.551 0.298 31.859)"",
    ""closest_hex_value"": ""#e11d48""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""700"",
    ""variable_name"": ""--color-rose-700"",
    ""oklch_value"": ""oklch(0.446 0.259 33.214)"",
    ""closest_hex_value"": ""#be123c""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""800"",
    ""variable_name"": ""--color-rose-800"",
    ""oklch_value"": ""oklch(0.375 0.215 34.569)"",
    ""closest_hex_value"": ""#9f1239""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""900"",
    ""variable_name"": ""--color-rose-900"",
    ""oklch_value"": ""oklch(0.314 0.174 35.923)"",
    ""closest_hex_value"": ""#881337""
  },
  {
    ""color_type"": ""rose"",
    ""color_int_value"": ""950"",
    ""variable_name"": ""--color-rose-950"",
    ""oklch_value"": ""oklch(0.258 0.131 37.278)"",
    ""closest_hex_value"": ""#4c051d""
  },
  {
    ""color_type"": ""black"",
    ""color_int_value"": null,
    ""variable_name"": ""--color-black"",
    ""oklch_value"": ""oklch(0% 0 0)"",
    ""closest_hex_value"": ""#000000""
  },
  {
    ""color_type"": ""white"",
    ""color_int_value"": null,
    ""variable_name"": ""--color-white"",
    ""oklch_value"": ""oklch(100% 0 0)"",
    ""closest_hex_value"": ""#ffffff""
  }
]
";
		var colorSB = new StringBuilder();
		colorSB.AppendLine("#region << Generated >>");

		var colorDefinitions = JsonSerializer.Deserialize<List<TfConvertersColor>>(colorJSON) ?? new List<TfConvertersColor>();
		colorDefinitions = colorDefinitions.OrderBy(x => x.ColorType).ToList();
		CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
		TextInfo textInfo = cultureInfo.TextInfo;
		var colorCountHash = new HashSet<string>();

		var nonSelectableList = new List<string> { "gray", "zinc", "neutral", "stone" };

		foreach (var def in colorDefinitions)
		{
			if (!colorCountHash.Contains(def.ColorType))
				colorCountHash.Add(def.ColorType);

			var colorCount = colorCountHash.Count;			
			
			var selectable = "false";
			if (!nonSelectableList.Any(x=> def.VariableName.Contains(x)))
				selectable = "true";

			colorSB.AppendLine($"[TfColor(name: \"{def.ColorType}\",variable:\"--tf-{def.ColorType}-{def.ColorIntValue}\",oklch:\"{def.OKLCH}\",hex:\"{def.HEX}\",number:{def.ColorIntValue}, selectable: {selectable})]");
			colorSB.AppendLine($"{textInfo.ToTitleCase(def.ColorType)}{def.ColorIntValue} = {colorCount}{def.ColorIntValue},");
		}

		colorSB.AppendLine("#endregion << Generated >>");

		var result = colorSB.ToString();
	}

	public static List<TfColor> GetSelectableColors()
	{
		return Enum.GetValues<TfColor>().Where(x => x.GetColor().Selectable).ToList();
	}

	#endregion

	#region << Exception >>
	public static List<string> GetDataAsErrorList(Exception ex)
	{
		List<string> result = new List<string>();
		if (ex is null || ex.Data is null)
			return result;
		foreach (var key in ex.Data.Keys)
		{
			if (ex.Data[key] is null) continue;

			if (ex.Data[key] is List<ValidationError>)
			{
				foreach (var valEx in (List<ValidationError>)ex.Data[key]!)
				{
					if (String.IsNullOrWhiteSpace(valEx.Message)) continue;
					result.Add(valEx.Message);
				}
			}
			if (ex.Data[key] is List<string>)
			{
				result.AddRange((List<string>)ex.Data[key]!);
			}
		}
		return result;
	}
	#endregion

	private class TfConvertersColor
	{
		[JsonPropertyName("color_type")]
		public string ColorType { get; set; }		
		
		[JsonPropertyName("color_int_value")]
		public string ColorIntValue { get; set; }			
		
		[JsonPropertyName("variable_name")]
		public string VariableName { get; set; }
		[JsonPropertyName("oklch_value")]
		public string OKLCH { get; set; }
		[JsonPropertyName("closest_hex_value")]
		public string HEX { get; set; }
	}	
}

