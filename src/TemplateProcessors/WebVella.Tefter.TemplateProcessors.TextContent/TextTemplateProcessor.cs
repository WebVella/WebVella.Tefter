namespace WebVella.Tefter.TemplateProcessors.TextContent;

using TfTemplate = WebVella.Tefter.Models.TfTemplate;

public class TextTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.TEXT_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter text content template";

	public string Description => "creates text content from a template and data";

	public string FluentIconName => "ScanText";

	public TfTemplateResultType ResultType => TfTemplateResultType.Text;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template, 
		TfDataTable data)
	{
		return null;
	}

	public List<string> GetUsedColumns(
		string settingsJson, 
		ITfTemplateService templateService)
	{
		List<string> usedColumns = new List<string>();

		//TODO implement

		return usedColumns;
	}

	public List<TfTemplate> GetUsedTemplates(
		string settingsJson,
		ITfTemplateService templateService)
	{
		return new List<TfTemplate>();
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		ITfTemplateService templateService)
	{
		throw new NotImplementedException();
	}

	public string OnCreate(
		string settingsJson,
		ITfTemplateService templateService)
	{
		return settingsJson;
	}

	public string OnUpdate(
		Guid templateId,
		string newSettingsJson,
		ITfTemplateService templateService)
	{
		return newSettingsJson;

	}

	public void OnDelete(
		Guid templateId,
		ITfTemplateService templateService)
	{
	}
}
