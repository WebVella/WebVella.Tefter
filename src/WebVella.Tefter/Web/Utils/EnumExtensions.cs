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
		if (String.IsNullOrEmpty(value)) return defaultValue;
		if (int.TryParse(value, out var n))
		{
			return ConvertIntToEnum(n, defaultValue);
		}
		return defaultValue;
	}

	public static TEnum2 ConvertSafeToEnum<TEnum, TEnum2>(this TEnum value) where TEnum : struct, IConvertible
																							 where TEnum2 : struct, IConvertible
	{
		if (Enum.TryParse<TEnum2>(value.ToString(), out TEnum2 result) && result.ToInt16(null) == value.ToInt16(null))
			return result;

		throw new Exception("Cannot be safely converted");

	}

	internal static string GetFluentIcon<TEnum>(this TEnum e) where TEnum : IConvertible
	{

		if (e is Enum)
		{
			Type type = e.GetType();
			var memInfo = type.GetMember(type.GetEnumName(e.ToInt32(CultureInfo.InvariantCulture)));
			var soAttributes = memInfo[0].GetCustomAttributes(typeof(FluentIconAttribute), false);
			if (soAttributes.Length > 0)
			{
				// we're only getting the first description we find
				// others will be ignored
				return ((FluentIconAttribute)soAttributes[0]).Name;
			}
		}

		return "ErrorCircle";
	}

	public static string ToColorString<TEnum>(this TEnum e) where TEnum : Enum
	{
		string description = "#fff";
		if (e is null)
			return description;
		if (e is Enum)
		{
			Type type = e.GetType();
			var memValue = type.GetEnumName((Enum)e);
			if(memValue is null)
				return description;
			var memInfo = type.GetMember(memValue);
			var soAttributes = memInfo[0].GetCustomAttributes(typeof(ColorAttribute), false);
			if (soAttributes.Length > 0)
			{
				// we're only getting the first description we find
				// others will be ignored
				description = ((ColorAttribute)soAttributes[0]).OKLCH;
			}
		}

		return description;
	}

	public static string ToColorString<TEnum>(this TEnum? e) where TEnum : struct, Enum
	{
		string description = "#fff";
		if (e is null)
			return description;
		if (e is Enum)
		{
			Type type = e.GetType();
			var memValue = type.GetEnumName((Enum)e.Value);
			if(memValue is null)
				return description;
			var memInfo = type.GetMember(memValue);
			var soAttributes = memInfo[0].GetCustomAttributes(typeof(ColorAttribute), false);
			if (soAttributes.Length > 0)
			{
				// we're only getting the first description we find
				// others will be ignored
				description = ((ColorAttribute)soAttributes[0]).OKLCH;
			}
		}

		return description;
	}

	public static string ToSelectableLabel<TEnum>(this TEnum e) where TEnum : IConvertible
	{
		string label = null;
		if (e is null)
			return label;
		if (e is Enum)
		{
			Type type = e.GetType();
			var memValue = type.GetEnumName(e.ToInt32(CultureInfo.InvariantCulture));
			if(memValue is null)
				return label;
			var memInfo = type.GetMember(memValue);
			var soAttributes = memInfo[0].GetCustomAttributes(typeof(SelectableAttribute), false);
			if (soAttributes.Length > 0)
			{
				// we're only getting the first description we find
				// others will be ignored
				label = ((SelectableAttribute)soAttributes[0]).Label;
			}
		}

		return label;
	}

	public static string ToName<TEnum>(this TEnum e) where TEnum : IConvertible
	{
		string label = null;
		if (e is null)
			return label;
		if (e is Enum)
		{
			Type type = e.GetType();
			var memValue = type.GetEnumName(e.ToInt32(CultureInfo.InvariantCulture));
			if(memValue is null)
				return label;
			var memInfo = type.GetMember(memValue);
			var soAttributes = memInfo[0].GetCustomAttributes(typeof(NameAttribute), false);
			if (soAttributes.Length > 0)
			{
				// we're only getting the first description we find
				// others will be ignored
				label = ((NameAttribute)soAttributes[0]).Name;
			}
		}

		return label;
	}

}
