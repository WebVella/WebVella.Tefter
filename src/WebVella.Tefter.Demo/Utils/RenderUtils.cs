namespace WebVella.Tefter.Demo.Utils;

public static class RenderUtils
{
	private static string conversionPrefix = "wv-";

	public static string ConvertGuidToHtmlElementId(Guid? guid)
	{
		if (guid == null) return null;
		return $"{conversionPrefix}{guid}";
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

	public static string StringOverflow(string input, int charCount)
	{
		if (input.Length <= charCount) return input;
		return input.Substring(0, charCount) + "...";
	}

}
