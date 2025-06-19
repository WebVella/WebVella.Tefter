namespace WebVella.Tefter.Utility;

public static class GuidExtensions
{
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
}