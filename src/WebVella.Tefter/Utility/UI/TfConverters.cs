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

	public static List<TfSort> ConvertQuerySortToList(this List<TfSortQuery>? items, List<TfSpaceViewColumn> columns)
	{
		var result = new List<TfSort>();
		if(items is null || columns is null || items.Count == 0 || columns.Count == 0) return result;

		foreach (var item in items)
		{
			var sort = item.ToSort(columns);
			if(sort is null) continue;
			result.Add(sort);
		}

		return result;
	}
	#endregion

	#region << Color >>
	public static void GenerateColorsFromVariables()
	{
		var colors = @"--color-red-50: oklch(97.1% .013 17.38);
    --color-red-100: oklch(93.6% .032 17.717);
    --color-red-200: oklch(88.5% .062 18.334);
    --color-red-300: oklch(80.8% .114 19.571);
    --color-red-400: oklch(70.4% .191 22.216);
    --color-red-500: oklch(63.7% .237 25.331);
    --color-red-600: oklch(57.7% .245 27.325);
    --color-red-700: oklch(50.5% .213 27.518);
    --color-red-800: oklch(44.4% .177 26.899);
    --color-red-900: oklch(39.6% .141 25.723);
    --color-red-950: oklch(25.8% .092 26.042);
    --color-orange-50: oklch(98% .016 73.684);
    --color-orange-100: oklch(95.4% .038 75.164);
    --color-orange-200: oklch(90.1% .076 70.697);
    --color-orange-300: oklch(83.7% .128 66.29);
    --color-orange-400: oklch(75% .183 55.934);
    --color-orange-500: oklch(70.5% .213 47.604);
    --color-orange-600: oklch(64.6% .222 41.116);
    --color-orange-700: oklch(55.3% .195 38.402);
    --color-orange-800: oklch(47% .157 37.304);
    --color-orange-900: oklch(40.8% .123 38.172);
    --color-orange-950: oklch(26.6% .079 36.259);
    --color-amber-50: oklch(98.7% .022 95.277);
    --color-amber-100: oklch(96.2% .059 95.617);
    --color-amber-200: oklch(92.4% .12 95.746);
    --color-amber-300: oklch(87.9% .169 91.605);
    --color-amber-400: oklch(82.8% .189 84.429);
    --color-amber-500: oklch(76.9% .188 70.08);
    --color-amber-600: oklch(66.6% .179 58.318);
    --color-amber-700: oklch(55.5% .163 48.998);
    --color-amber-800: oklch(47.3% .137 46.201);
    --color-amber-900: oklch(41.4% .112 45.904);
    --color-amber-950: oklch(27.9% .077 45.635);
    --color-yellow-50: oklch(98.7% .026 102.212);
    --color-yellow-100: oklch(97.3% .071 103.193);
    --color-yellow-200: oklch(94.5% .129 101.54);
    --color-yellow-300: oklch(90.5% .182 98.111);
    --color-yellow-400: oklch(85.2% .199 91.936);
    --color-yellow-500: oklch(79.5% .184 86.047);
    --color-yellow-600: oklch(68.1% .162 75.834);
    --color-yellow-700: oklch(55.4% .135 66.442);
    --color-yellow-800: oklch(47.6% .114 61.907);
    --color-yellow-900: oklch(42.1% .095 57.708);
    --color-yellow-950: oklch(28.6% .066 53.813);
    --color-lime-50: oklch(98.6% .031 120.757);
    --color-lime-100: oklch(96.7% .067 122.328);
    --color-lime-200: oklch(93.8% .127 124.321);
    --color-lime-300: oklch(89.7% .196 126.665);
    --color-lime-400: oklch(84.1% .238 128.85);
    --color-lime-500: oklch(76.8% .233 130.85);
    --color-lime-600: oklch(64.8% .2 131.684);
    --color-lime-700: oklch(53.2% .157 131.589);
    --color-lime-800: oklch(45.3% .124 130.933);
    --color-lime-900: oklch(40.5% .101 131.063);
    --color-lime-950: oklch(27.4% .072 132.109);
    --color-green-50: oklch(98.2% .018 155.826);
    --color-green-100: oklch(96.2% .044 156.743);
    --color-green-200: oklch(92.5% .084 155.995);
    --color-green-300: oklch(87.1% .15 154.449);
    --color-green-400: oklch(79.2% .209 151.711);
    --color-green-500: oklch(72.3% .219 149.579);
    --color-green-600: oklch(62.7% .194 149.214);
    --color-green-700: oklch(52.7% .154 150.069);
    --color-green-800: oklch(44.8% .119 151.328);
    --color-green-900: oklch(39.3% .095 152.535);
    --color-green-950: oklch(26.6% .065 152.934);
    --color-emerald-50: oklch(97.9% .021 166.113);
    --color-emerald-100: oklch(95% .052 163.051);
    --color-emerald-200: oklch(90.5% .093 164.15);
    --color-emerald-300: oklch(84.5% .143 164.978);
    --color-emerald-400: oklch(76.5% .177 163.223);
    --color-emerald-500: oklch(69.6% .17 162.48);
    --color-emerald-600: oklch(59.6% .145 163.225);
    --color-emerald-700: oklch(50.8% .118 165.612);
    --color-emerald-800: oklch(43.2% .095 166.913);
    --color-emerald-900: oklch(37.8% .077 168.94);
    --color-emerald-950: oklch(26.2% .051 172.552);
    --color-teal-50: oklch(98.4% .014 180.72);
    --color-teal-100: oklch(95.3% .051 180.801);
    --color-teal-200: oklch(91% .096 180.426);
    --color-teal-300: oklch(85.5% .138 181.071);
    --color-teal-400: oklch(77.7% .152 181.912);
    --color-teal-500: oklch(70.4% .14 182.503);
    --color-teal-600: oklch(60% .118 184.704);
    --color-teal-700: oklch(51.1% .096 186.391);
    --color-teal-800: oklch(43.7% .078 188.216);
    --color-teal-900: oklch(38.6% .063 188.416);
    --color-teal-950: oklch(27.7% .046 192.524);
    --color-cyan-50: oklch(98.4% .019 200.873);
    --color-cyan-100: oklch(95.6% .045 203.388);
    --color-cyan-200: oklch(91.7% .08 205.041);
    --color-cyan-300: oklch(86.5% .127 207.078);
    --color-cyan-400: oklch(78.9% .154 211.53);
    --color-cyan-500: oklch(71.5% .143 215.221);
    --color-cyan-600: oklch(60.9% .126 221.723);
    --color-cyan-700: oklch(52% .105 223.128);
    --color-cyan-800: oklch(45% .085 224.283);
    --color-cyan-900: oklch(39.8% .07 227.392);
    --color-cyan-950: oklch(30.2% .056 229.695);
    --color-sky-50: oklch(97.7% .013 236.62);
    --color-sky-100: oklch(95.1% .026 236.824);
    --color-sky-200: oklch(90.1% .058 230.902);
    --color-sky-300: oklch(82.8% .111 230.318);
    --color-sky-400: oklch(74.6% .16 232.661);
    --color-sky-500: oklch(68.5% .169 237.323);
    --color-sky-600: oklch(58.8% .158 241.966);
    --color-sky-700: oklch(50% .134 242.749);
    --color-sky-800: oklch(44.3% .11 240.79);
    --color-sky-900: oklch(39.1% .09 240.876);
    --color-sky-950: oklch(29.3% .066 243.157);
    --color-blue-50: oklch(97% .014 254.604);
    --color-blue-100: oklch(93.2% .032 255.585);
    --color-blue-200: oklch(88.2% .059 254.128);
    --color-blue-300: oklch(80.9% .105 251.813);
    --color-blue-400: oklch(70.7% .165 254.624);
    --color-blue-500: oklch(62.3% .214 259.815);
    --color-blue-600: oklch(54.6% .245 262.881);
    --color-blue-700: oklch(48.8% .243 264.376);
    --color-blue-800: oklch(42.4% .199 265.638);
    --color-blue-900: oklch(37.9% .146 265.522);
    --color-blue-950: oklch(28.2% .091 267.935);
    --color-indigo-50: oklch(96.2% .018 272.314);
    --color-indigo-100: oklch(93% .034 272.788);
    --color-indigo-200: oklch(87% .065 274.039);
    --color-indigo-300: oklch(78.5% .115 274.713);
    --color-indigo-400: oklch(67.3% .182 276.935);
    --color-indigo-500: oklch(58.5% .233 277.117);
    --color-indigo-600: oklch(51.1% .262 276.966);
    --color-indigo-700: oklch(45.7% .24 277.023);
    --color-indigo-800: oklch(39.8% .195 277.366);
    --color-indigo-900: oklch(35.9% .144 278.697);
    --color-indigo-950: oklch(25.7% .09 281.288);
    --color-violet-50: oklch(96.9% .016 293.756);
    --color-violet-100: oklch(94.3% .029 294.588);
    --color-violet-200: oklch(89.4% .057 293.283);
    --color-violet-300: oklch(81.1% .111 293.571);
    --color-violet-400: oklch(70.2% .183 293.541);
    --color-violet-500: oklch(60.6% .25 292.717);
    --color-violet-600: oklch(54.1% .281 293.009);
    --color-violet-700: oklch(49.1% .27 292.581);
    --color-violet-800: oklch(43.2% .232 292.759);
    --color-violet-900: oklch(38% .189 293.745);
    --color-violet-950: oklch(28.3% .141 291.089);
    --color-purple-50: oklch(97.7% .014 308.299);
    --color-purple-100: oklch(94.6% .033 307.174);
    --color-purple-200: oklch(90.2% .063 306.703);
    --color-purple-300: oklch(82.7% .119 306.383);
    --color-purple-400: oklch(71.4% .203 305.504);
    --color-purple-500: oklch(62.7% .265 303.9);
    --color-purple-600: oklch(55.8% .288 302.321);
    --color-purple-700: oklch(49.6% .265 301.924);
    --color-purple-800: oklch(43.8% .218 303.724);
    --color-purple-900: oklch(38.1% .176 304.987);
    --color-purple-950: oklch(29.1% .149 302.717);
    --color-fuchsia-50: oklch(97.7% .017 320.058);
    --color-fuchsia-100: oklch(95.2% .037 318.852);
    --color-fuchsia-200: oklch(90.3% .076 319.62);
    --color-fuchsia-300: oklch(83.3% .145 321.434);
    --color-fuchsia-400: oklch(74% .238 322.16);
    --color-fuchsia-500: oklch(66.7% .295 322.15);
    --color-fuchsia-600: oklch(59.1% .293 322.896);
    --color-fuchsia-700: oklch(51.8% .253 323.949);
    --color-fuchsia-800: oklch(45.2% .211 324.591);
    --color-fuchsia-900: oklch(40.1% .17 325.612);
    --color-fuchsia-950: oklch(29.3% .136 325.661);
    --color-pink-50: oklch(97.1% .014 343.198);
    --color-pink-100: oklch(94.8% .028 342.258);
    --color-pink-200: oklch(89.9% .061 343.231);
    --color-pink-300: oklch(82.3% .12 346.018);
    --color-pink-400: oklch(71.8% .202 349.761);
    --color-pink-500: oklch(65.6% .241 354.308);
    --color-pink-600: oklch(59.2% .249 .584);
    --color-pink-700: oklch(52.5% .223 3.958);
    --color-pink-800: oklch(45.9% .187 3.815);
    --color-pink-900: oklch(40.8% .153 2.432);
    --color-pink-950: oklch(28.4% .109 3.907);
    --color-rose-50: oklch(96.9% .015 12.422);
    --color-rose-100: oklch(94.1% .03 12.58);
    --color-rose-200: oklch(89.2% .058 10.001);
    --color-rose-300: oklch(81% .117 11.638);
    --color-rose-400: oklch(71.2% .194 13.428);
    --color-rose-500: oklch(64.5% .246 16.439);
    --color-rose-600: oklch(58.6% .253 17.585);
    --color-rose-700: oklch(51.4% .222 16.935);
    --color-rose-800: oklch(45.5% .188 13.697);
    --color-rose-900: oklch(41% .159 10.272);
    --color-rose-950: oklch(27.1% .105 12.094);
    --color-slate-50: oklch(98.4% .003 247.858);
    --color-slate-100: oklch(96.8% .007 247.896);
    --color-slate-200: oklch(92.9% .013 255.508);
    --color-slate-300: oklch(86.9% .022 252.894);
    --color-slate-400: oklch(70.4% .04 256.788);
    --color-slate-500: oklch(55.4% .046 257.417);
    --color-slate-600: oklch(44.6% .043 257.281);
    --color-slate-700: oklch(37.2% .044 257.287);
    --color-slate-800: oklch(27.9% .041 260.031);
    --color-slate-900: oklch(20.8% .042 265.755);
    --color-slate-950: oklch(12.9% .042 264.695);
    --color-gray-50: oklch(98.5% .002 247.839);
    --color-gray-100: oklch(96.7% .003 264.542);
    --color-gray-200: oklch(92.8% .006 264.531);
    --color-gray-300: oklch(87.2% .01 258.338);
    --color-gray-400: oklch(70.7% .022 261.325);
    --color-gray-500: oklch(55.1% .027 264.364);
    --color-gray-600: oklch(44.6% .03 256.802);
    --color-gray-700: oklch(37.3% .034 259.733);
    --color-gray-800: oklch(27.8% .033 256.848);
    --color-gray-900: oklch(21% .034 264.665);
    --color-gray-950: oklch(13% .028 261.692);
    --color-zinc-50: oklch(98.5% 0 0);
    --color-zinc-100: oklch(96.7% .001 286.375);
    --color-zinc-200: oklch(92% .004 286.32);
    --color-zinc-300: oklch(87.1% .006 286.286);
    --color-zinc-400: oklch(70.5% .015 286.067);
    --color-zinc-500: oklch(55.2% .016 285.938);
    --color-zinc-600: oklch(44.2% .017 285.786);
    --color-zinc-700: oklch(37% .013 285.805);
    --color-zinc-800: oklch(27.4% .006 286.033);
    --color-zinc-900: oklch(21% .006 285.885);
    --color-zinc-950: oklch(14.1% .005 285.823);
    --color-neutral-50: oklch(98.5% 0 0);
    --color-neutral-100: oklch(97% 0 0);
    --color-neutral-200: oklch(92.2% 0 0);
    --color-neutral-300: oklch(87% 0 0);
    --color-neutral-400: oklch(70.8% 0 0);
    --color-neutral-500: oklch(55.6% 0 0);
    --color-neutral-600: oklch(43.9% 0 0);
    --color-neutral-700: oklch(37.1% 0 0);
    --color-neutral-800: oklch(26.9% 0 0);
    --color-neutral-900: oklch(20.5% 0 0);
    --color-neutral-950: oklch(14.5% 0 0);
    --color-stone-50: oklch(98.5% .001 106.423);
    --color-stone-100: oklch(97% .001 106.424);
    --color-stone-200: oklch(92.3% .003 48.717);
    --color-stone-300: oklch(86.9% .005 56.366);
    --color-stone-400: oklch(70.9% .01 56.259);
    --color-stone-500: oklch(55.3% .013 58.071);
    --color-stone-600: oklch(44.4% .011 73.639);
    --color-stone-700: oklch(37.4% .01 67.558);
    --color-stone-800: oklch(26.8% .007 34.298);
    --color-stone-900: oklch(21.6% .006 56.043);
    --color-stone-950: oklch(14.7% .004 49.25);";
		var colorSB = new StringBuilder();
		colorSB.AppendLine("#region << Generated >>");

		var colorDefinitions = colors.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
		CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
		TextInfo textInfo = cultureInfo.TextInfo;
		var colorCountHash = new HashSet<string>();

		var nonSelectableList = new List<string> { "gray", "zinc", "neutral", "stone" };


		foreach (var def in colorDefinitions)
		{
			var defArray = def.Trim().ToLower().Replace(";", "").Replace("--color-", "").Split(":", StringSplitOptions.RemoveEmptyEntries);
			if (defArray.Length < 2) continue;

			var colorValue = defArray[1].Trim();
			var colorNameArray = defArray[0].Split("-", StringSplitOptions.RemoveEmptyEntries);
			var colorType = colorNameArray[0].Trim();
			int colorNumber = int.Parse(colorNameArray[1].Trim());
			if (!colorCountHash.Contains(colorType))
				colorCountHash.Add(colorType);

			var colorCount = colorCountHash.Count;
			var selectable = "false";
			if (colorNumber == 500 && !nonSelectableList.Contains(colorType))
				selectable = "true";

			colorSB.AppendLine($"[TfColor(name: \"{colorType}\",value:\"{colorValue}\",variable:\"--tf-{defArray[0]}\",number:{colorNumber}, selectable: {selectable})]");
			colorSB.AppendLine($"{textInfo.ToTitleCase(colorType)}{colorNumber} = {colorCount}{colorNumber},");
		}

		colorSB.AppendLine("#endregion << Generated >>");

		var result = colorSB.ToString();
	}

	public static List<TfColor> GetSelectableColors()
	{
		return Enum.GetValues<TfColor>().Where(x => x.GetAttribute().Selectable).ToList();
	}

	#endregion

	#region << Exception >>
	public static List<string> GetDataAsErrorList(Exception ex)
	{
		List<string> result = new List<string>();
		if(ex is null || ex.Data is null)
			return result;
		foreach (var key in ex.Data.Keys)
		{
			if (ex.Data[key] is null) continue;

			if (ex.Data[key] is List<ValidationError>)
			{
				foreach (var valEx in (List<ValidationError>)ex.Data[key]!)
				{
					if(String.IsNullOrWhiteSpace(valEx.Message)) continue;
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
}
