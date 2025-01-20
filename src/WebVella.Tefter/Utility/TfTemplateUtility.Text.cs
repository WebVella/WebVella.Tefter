namespace WebVella.Tefter.Utility;
public static partial class TfTemplateUtility
{
	public static void ProcessTextTemplate(this TfTextTemplateProcessResult result,
		TfDataTable dataSource, CultureInfo culture = null)
	{
		if (culture == null) culture = new CultureInfo("en-US");
		if (result is null) throw new Exception("No result provided!");
		if (dataSource is null) throw new Exception("No datasource provided!");
		if(String.IsNullOrWhiteSpace(result.TemplateText))
		{ 
			result.ResultText = result.TemplateText;
			return;
		}
		var sb = new StringBuilder();
		var lines = result.TemplateText.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		var endWithNewLine = result.TemplateText.EndsWith(Environment.NewLine);
		if (lines.Count() == 1)
		{
			var tagProcessResult = ProcessTemplateTag(lines.First(), dataSource, culture);
			if (tagProcessResult.Values.Count == 1)
			{
				sb.Append(tagProcessResult.Values[0]?.ToString());
			}
			else if (tagProcessResult.Values.Count > 1)
			{
				foreach (var value in tagProcessResult.Values)
				{
					if(endWithNewLine)
						sb.AppendLine(value?.ToString());
					else
						sb.Append(value?.ToString());
				}
			}
		}
		else if (lines.Count() > 1)
		{
			foreach (string line in lines)
			{
				var tagProcessResult = ProcessTemplateTag(line, dataSource, culture);
				foreach (var value in tagProcessResult.Values)
				{
					sb.AppendLine(value?.ToString());
				}
			}
		}
		result.ResultText = sb.ToString();
	}

}
