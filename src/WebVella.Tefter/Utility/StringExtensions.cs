namespace WebVella.Tefter.Utility;

public static class StringExtensions
{
	//utility method to get configuration file path
	public static string ToApplicationPath(this string fileName)
	{
		var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
		var appRoot = appPathMatcher.Match(exePath).Value;
		return Path.Combine(appRoot, fileName);
	}

	public static string ToMD5Hash(this string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return string.Empty;

		byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

		StringBuilder sBuilder = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
			sBuilder.Append(data[i].ToString("x2"));

		return sBuilder.ToString();
	}

	public static bool IsEmail(this string text)
	{
		try
		{
			return new MailAddress(text).Address == text;
		}
		catch
		{
			return false;
		}
	}

	public static bool IsGreaterThan(this string? i, string? value)
	{
		if (i is null && value is null) return false;
		if (i is null) return false;
		return (i.CompareTo(value) > 0);
	}

	public static bool IsLessThan(this string? i, string? value)
	{
		if( i is null && value is null) return false;
		if (i is null) return true;
		return (i.CompareTo(value) < 0);
	}

	/// <summary>
	/// Tries to convert the supplied string into a <see cref="long"/>.
	/// </summary>
	/// <param name="source">The source string (may be null or empty).</param>
	/// <returns>A nullable long – <c>null</c> if the string cannot be parsed.</returns>
	public static long? ToNullableLong(this string? source)
	{
		// Fast path for null / whitespace
		if (string.IsNullOrWhiteSpace(source))
			return null;

		// Try to parse using the default culture‑neutral overload.
		// You can replace this with a specific CultureInfo if needed.
		if (long.TryParse(source, out var result))
			return result;

		// If parsing fails, we simply return null instead of throwing
		return null;
	}

	/// <summary>
	/// Tries to convert the supplied string into a <see cref="decimal"/>.
	/// </summary>
	/// <param name="source">The source string (may be null or empty).</param>
	/// <param name="formatProvider">
	/// Optional format provider.  If omitted, <c>CultureInfo.InvariantCulture</c> is used.
	/// </param>
	/// <returns>A nullable decimal – <c>null</c> if parsing fails.</returns>
	public static decimal? ToNullableDecimal(
		this string? source,
		IFormatProvider formatProvider = null)
	{
		// 1️⃣ Guard against null/whitespace
		if (string.IsNullOrWhiteSpace(source))
			return null;

		// 2️⃣ Pick a safe default culture
		var provider = formatProvider ?? TfConstants.DefaultCulture;

		// 3️⃣ Try to parse using the supplied or default culture.
		//    We use NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint
		//    to support common numeric formats (e.g., "1,234.56").
		if (decimal.TryParse(
				source,
				NumberStyles.Number,
				provider,
				out var result))
		{
			return result;
		}

		// 4️⃣ Parsing failed – return null.
		return null;
	}
	
	/// <summary>
    /// Validates a string to ensure it only contains printable Unicode characters 
    /// and does not contain unpaired surrogate code points.
    /// </summary>
    /// <param name="text">The string to validate.</param>
    /// <returns>True if the string contains only valid, printable characters; otherwise, False.</returns>
    public static bool ContainsOnlyPrintableAndValidChars(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return true;
        }

        // --- 1. Check for Non-Printable/Control Characters ---

        // \p{C} is the 'Other' category, which includes:
        // - \p{Cc} (Control characters, e.g., \0, \n, \t)
        // - \p{Cf} (Format characters, e.g., Right-to-Left marks)
        // - \p{Cs} (Surrogates, checked more strictly below, but good as a first pass)
        // - \p{Co} (Private Use)
        // - \p{Cn} (Not Assigned)
        // We generally want to exclude the Control Characters (\p{Cc}). 
        // We'll use a specific check for control chars for clarity and completeness.

        // Check for common non-printable control characters (\p{Cc}) 
        // *excluding* tabs (\t, U+0009) and newlines (\n, U+000A) which might be allowed 
        // in some contexts, but this method excludes them by default.
        // We use LINQ's Char.IsControl method.
        if (text.Any(c => char.IsControl(c) && c != '\r' && c != '\n' && c != '\t'))
        {
            return false;
        }

        // --- 2. Check for Invalid Unicode Code Points (Unpaired Surrogates) ---

        // In C#, a string is UTF-16. 'Non-UTF-8' often refers to invalid byte sequences.
        // In the context of a C# string, the closest equivalent to "invalid character" 
        // is an **unpaired surrogate**. A surrogate pair (two char U+D800-U+DFFF) is 
        // used to represent characters outside the Basic Multilingual Plane (BMP, U+0000-U+FFFF).
        // A *lone* surrogate is an invalid code point.

        // We use a regular expression to detect unpaired surrogates.
        // It looks for a high surrogate not followed by a low surrogate, 
        // OR a low surrogate not preceded by a high surrogate.

        // The pattern is: 
        // '[\uD800-\uDBFF](?![\uDC00-\uDFFF])'  <-- High surrogate NOT followed by low surrogate
        // '|'                                  <-- OR
        // '(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]' <-- Low surrogate NOT preceded by high surrogate
        string unpairedSurrogatePattern = @"[\uD800-\uDBFF](?![\uDC00-\uDFFF])|(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]";

        if (Regex.IsMatch(text, unpairedSurrogatePattern))
        {
            return false;
        }

        return true;
    }	
}