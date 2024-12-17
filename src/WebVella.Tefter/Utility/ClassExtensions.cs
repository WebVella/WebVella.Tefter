namespace WebVella.Tefter.Utility;
public static class ClassExtensions
{
	internal static string ToDescriptionString(this Type type)
	{

		var descriptions = (DescriptionAttribute[])
				   type.GetCustomAttributes(typeof(DescriptionAttribute), false);

		if (descriptions.Length > 0) return descriptions[0].Description;

		return type.Name;
	}

	internal static (string,string) GetLocalizationResourceInfo(this Type type)
	{

		var attributes = (LocalizationResourceAttribute[])
				   type.GetCustomAttributes(typeof(LocalizationResourceAttribute), false);

		if (attributes.Length > 0) return (attributes[0].BaseName,attributes[0].Location);

		return (null,null);
	}

	internal static string GetFluentIcon(this Type type)
	{

		var attributes = (FluentIconAttribute[])
				   type.GetCustomAttributes(typeof(FluentIconAttribute), false);

		if (attributes.Length > 0) return attributes[0].Name;

		return "ErrorCircle";
	}
}
