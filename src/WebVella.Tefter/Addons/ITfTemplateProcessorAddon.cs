namespace WebVella.Tefter.Models;

public interface ITfTemplateProcessorAddon
{
	public Guid Id { get; init;}
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init;}
	public TfTemplateResultType ResultType { get; init;}

	public void ValidatePreview(
		TfTemplate template,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider);

	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		TfTemplate template,
		TfDataTable dataTable,
		List<Guid> tfRecordIds,
		List<Guid> tfDatasetIds,
		List<Guid> tfSpaceIds,	
		Guid sessionId,
		Guid userId,
		IServiceProvider serviceProvider);

	public ITfTemplateResult ProcessTemplate(
		TfTemplate template,
		TfDataTable dataTable,
		List<Guid> tfRecordIds,
		List<Guid> tfDatasetIds,
		List<Guid> tfSpaceIds,	
		Guid sessionId,
		Guid userId,
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