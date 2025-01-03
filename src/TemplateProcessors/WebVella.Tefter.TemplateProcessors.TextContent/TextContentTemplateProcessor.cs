using WebVella.Tefter.TemplateProcessors.TextContent.Components;

namespace WebVella.Tefter.TemplateProcessors.TextContent;

public class TextContentTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.TEXT_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter text content template";
	public string Description => "creates text content from a template and data";
	public string FluentIconName => "ScanText";
	public TfTemplateResultType ResultType => TfTemplateResultType.Text;
	public Type SettingsComponentType => typeof(SettingsComponent);
	public Type ResultPreviewComponentType => typeof(ResultPreviewComponent);
	public Type ResultComponentType => typeof(ResultComponent);
	public Type HelpComponentType => typeof(HelpComponent);

	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		TfTemplate template,
		TfSpace tfSpace,
		List<Guid> tfRecordIds,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return null;
	}

	public ITfTemplateResult ProcessTemplate(
		TfTemplate template,
		TfSpace tfSpace,
		List<Guid> tfRecordIds,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
		//TODO
		return null;
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
