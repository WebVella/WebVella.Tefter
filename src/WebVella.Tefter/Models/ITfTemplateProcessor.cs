namespace WebVella.Tefter.Models;

public interface ITfTemplateProcessor
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public string FluentIconName { get; }
	public TfTemplateResultType ResultType { get; }
	public Type SettingsComponentType { get; }
	public Type ResultPreviewComponentType { get; }
	public Type ResultComponentType { get; }
	public Type HelpComponentType { get; }

	public void ValidatePreview(
		TfTemplate template,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider);

	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		TfTemplate template,
		TfDataTable dataTable,
		IServiceProvider serviceProvider);

	public ITfTemplateResult ProcessTemplate(
		TfTemplate template,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider);

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider );

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider );

	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider);

	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider);

	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider);

	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider);

	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider);
}