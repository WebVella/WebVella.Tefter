using System;
using System.Text.RegularExpressions;

namespace LOCResourceExtractor;
public static class LocExtractor
{
	public static List<string> ExtractLocValues(string content)
	{
		var values = new List<string>();
		var pattern = @"LOC\(""(.*?)""\)|LOC\[""(.*?)""\]";
		var matches = Regex.Matches(content, pattern);

		foreach (Match match in matches)
		{
			// The value could be in either group 1 or group 2, but not both for a single match.
			// We check both and add the non-empty one.
			if (match.Groups[1].Success)
			{
				values.Add(match.Groups[1].Value);
			}
			else if (match.Groups[2].Success)
			{
				values.Add(match.Groups[2].Value);
			}
		}
		return values;
	}
}