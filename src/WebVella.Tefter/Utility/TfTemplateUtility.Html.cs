using HtmlAgilityPack;

namespace WebVella.Tefter.Utility;
public static partial class TfTemplateUtility
{
	public static void ProcessHtmlTemplate(this TfHtmlTemplateProcessResult result,
		TfDataTable dataSource, CultureInfo culture = null)
	{
		if (culture == null) culture = new CultureInfo("en-US");
		if (result is null) throw new Exception("No result provided!");
		if (dataSource is null) throw new Exception("No datasource provided!");
		if (String.IsNullOrWhiteSpace(result.TemplateHtml))
		{
			result.ResultHtml = result.TemplateHtml;
			return;
		}
		//The editor encodes some of the symbols like ' and ""
		result.TemplateHtml = HttpUtility.HtmlDecode(result.TemplateHtml);

		HtmlDocument doc = new HtmlDocument();
		doc.OptionEmptyCollection = true; //Prevent from returning null
		HtmlDocument docResult = new HtmlDocument();
		doc.LoadHtml(result.TemplateHtml);

		foreach (HtmlNode node in doc.DocumentNode.ChildNodes)
		{
			var nameLowered = node.Name.ToLowerInvariant();

			var tagProcessResult = ProcessTemplateTag(node.InnerHtml, dataSource, culture);
			foreach (var value in tagProcessResult.Values)
			{
				if(node.NodeType == HtmlNodeType.Text){ 
					var textResult = docResult.CreateTextNode();
					textResult.InnerHtml = value.ToString();
					docResult.DocumentNode.AppendChild(textResult);				
				}
				else
				{
					var divResult = docResult.CreateElement(node.Name);
					divResult.InnerHtml = value.ToString();
					docResult.DocumentNode.AppendChild(divResult);
				}
			}
		}

		result.ResultHtml = docResult.DocumentNode.InnerHtml;
	}

}
