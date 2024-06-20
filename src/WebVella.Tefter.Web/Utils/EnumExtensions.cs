namespace WebVella.Tefter.Web.Utils;

public static class EnumExtensions
{
	public static string ToDescriptionString<TEnum>(this TEnum e) where TEnum : IConvertible
	{
		string description = "";

		if (e is Enum)
		{
			Type type = e.GetType();
			var memInfo = type.GetMember(type.GetEnumName(e.ToInt32(CultureInfo.InvariantCulture)));
			var soAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (soAttributes.Length > 0)
			{
				// we're only getting the first description we find
				// others will be ignored
				description = ((DescriptionAttribute)soAttributes[0]).Description;
			}
		}

		return description;
	}

	public static TEnum ConvertIntToEnum<TEnum>(int value, TEnum defaultValue) where TEnum : IConvertible
	{
		if (Enum.IsDefined(typeof(TEnum), value))
		{
			return (TEnum)Enum.ToObject(typeof(TEnum), value);
		}
		return defaultValue;

	}

	public static TEnum ConvertStringToEnum<TEnum>(string value, TEnum defaultValue) where TEnum : IConvertible
	{
		if(String.IsNullOrEmpty(value)) return defaultValue;
		if(int.TryParse(value, out var n)){ 
			return ConvertIntToEnum(n, defaultValue);
		}
		return defaultValue;
	}

}
