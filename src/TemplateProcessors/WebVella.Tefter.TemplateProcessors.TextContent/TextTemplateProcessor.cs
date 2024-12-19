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

	public List<string> GetUsedColumns(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return new List<string>();
	}

	public List<TfTemplate> GetUsedTemplates(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return new List<TfTemplate>();
	}

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return null;
	}

	public void ProcessTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
	}

	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
	}

	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
	}
}
