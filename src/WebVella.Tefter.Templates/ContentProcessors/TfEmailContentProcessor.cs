namespace WebVella.Tefter.Templates.ContentProcessors;

public class TfEmailContentProcessor : ITemplateProcessor
{
	public string Name => "Default tefter email content processor";

	public string Description => "Default tefter email content processor";

	public TemplateResultType ResultType => TemplateResultType.Email;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;

	public TemplateResult GenerateContent(string settingsJson, TfDataTable data)
	{
		return null;
	}

	public List<string> GetUsedCollumns(string settingsJson)
	{
		return new List<string>();	
	}
}
