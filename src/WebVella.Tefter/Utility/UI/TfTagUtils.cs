namespace WebVella.Tefter.Utility;

public static partial class TfTagUtils
{
	public static List<string> GetUniqueTagsFromText(
		this string? text)
	{
		var result = new List<string>();
		if(String.IsNullOrWhiteSpace(text)) return result;
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

}
