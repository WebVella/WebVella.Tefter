namespace WebVella.Tefter.Templates.ContentProcessors;

public class TfFileContentProcessor : ITemplateProcessor
{
	public string Name => "Default tefter file content processor";

	public string Description => "Default file email content processor";

	public TemplateResultType ResultType => TemplateResultType.File;

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
	public List<string> ValidateSettings(string settingsJson)
	{
		//validate settings for circular usage
		return new List<string>();
	}
}
