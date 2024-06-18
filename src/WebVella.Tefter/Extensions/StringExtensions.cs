namespace WebVella.Tefter;

public static class StringExtensions
{
    //utility method to get configuration file path
    public static string ToApplicationPath(this string fileName)
    {
        var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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
}