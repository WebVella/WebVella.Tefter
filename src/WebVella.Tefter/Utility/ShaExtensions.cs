namespace WebVella.Tefter.Utility;

public static class ShaExtensions
{
	static string ToSha1(this string? text)
	{
		using (SHA1 sha1 = SHA1.Create())
		{
			string input = text??string.Empty;
			byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder sb = new StringBuilder();
			foreach (byte b in hashBytes)
			{
				sb.Append(b.ToString("x2")); 
			}
			return sb.ToString();
		}
	}

	static string ToSha1(this Guid? guid)
	{
		using (SHA1 sha1 = SHA1.Create())
		{
			string input = guid.HasValue ? guid.Value.ToString() : string.Empty;
			byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder sb = new StringBuilder();
			foreach (byte b in hashBytes)
			{
				sb.Append(b.ToString("x2")); // Convert to hexadecimal
			}
			return sb.ToString();
		}
	}

	public static string ToSha1(this Guid guid)
	{
		using (SHA1 sha1 = SHA1.Create())
		{
			string input = guid.ToString();
			byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder sb = new StringBuilder();
			foreach (byte b in hashBytes)
			{
				sb.Append(b.ToString("x2")); // Convert to hexadecimal
			}
			return sb.ToString();
		}
	}

	public static bool IsSha1(this string value)
	{
		if (string.IsNullOrEmpty(value) || value.Length != 40)
			return false;
		
		foreach (char c in value)
		{
			if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
				return false;
		}

		return true;
	}
}