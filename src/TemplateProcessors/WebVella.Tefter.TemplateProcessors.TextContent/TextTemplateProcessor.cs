namespace WebVella.Tefter.TemplateProcessors.TextContent;

public class TextTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.TEXT_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter text content template";

	public string Description => "creates text content from a template and data";

	public string FluentIconName => "ScanText";

	public TfTemplateResultType ResultType => TfTemplateResultType.Text;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;
	public Type HelpComponentType => null;

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return null;
	}

	public List<ValidationError> ProcessTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}
}
