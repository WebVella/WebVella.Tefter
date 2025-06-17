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

	public static bool IsGreaterThan(this string i, string value)
	{
		return (i.CompareTo(value) > 0);
	}

	public static bool IsLessThan(this string i, string value)
	{
		return (i.CompareTo(value) < 0);
	}
}