namespace WebVella.Tefter.Utility;
public static partial class TfTemplateUtility
{
	public static void ProcessTextTemplate(this TfTextTemplateProcessResult result,
		TfDataTable dataSource, CultureInfo culture = null)
	{
		if (culture == null) culture = new CultureInfo("en-US");
		if (result is null) throw new Exception("No result provided!");
		if (dataSource is null) throw new Exception("No datasource provided!");
		var sb = new StringBuilder();
		foreach (string line in result.TemplateText.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
		{
			var tagProcessResult = ProcessTemplateTag(line, dataSource, culture);
			foreach (var value in tagProcessResult.Values)
			{
				sb.AppendLine(value?.ToString());
			}
		}
		result.ResultText = sb.ToString();
	}

}
