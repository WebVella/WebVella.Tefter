
namespace WebVella.Tefter.Models;

public interface ITfTemplateProcessor
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public string FluentIconName { get; }
	public TfTemplateResultType ResultType { get; }
	public Type SettingsComponentType { get; }
	public Type ResultViewComponentType { get; }

	public List<string> GetUsedColumns(
		string settingsJson,
		ITfTemplateService templateService);

	public List<TfTemplate> GetUsedTemplates(
		string settingsJson,
		ITfTemplateService templateService);

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data);

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		ITfTemplateService templateService);

	public string OnCreate(
		string settingsJson,
		ITfTemplateService templateService);

	public string OnUpdate(
		Guid templateId,
		string newSettingsJson,
		ITfTemplateService templateService);

	public void OnDelete(
		Guid templateId,
		ITfTemplateService templateService);


}