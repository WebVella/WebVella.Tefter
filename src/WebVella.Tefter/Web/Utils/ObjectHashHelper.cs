using System.Text;

namespace WebVella.Tefter.Web.Utils;

public static class ObjectHashHelper
{
	public static string CalculateHash<T>(T obj)
	{
		if (obj == null)
		{
			return null;
		}
		var json = JsonSerializer.Serialize(obj);
		var hash = "";
		using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
		{
			hash = BitConverter.ToString(
			  md5.ComputeHash(Encoding.UTF8.GetBytes(json))
			).Replace("-", String.Empty);
		}

		return hash;

	}


}