namespace WebVella.Tefter.Templates.Models;

public interface ITemplateProcessor
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public string FluentIconName { get; }
	public TemplateResultType ResultType { get; }
	public Type SettingsComponentType { get; }
	public Type ResultViewComponentType { get; }

	public List<string> GetUsedColumns(
		string settingsJson,
		ITemplatesService templateService);

	public List<Template> GetUsedTemplates(
		string settingsJson,
		ITemplatesService templateService);

	public ITemplateResult GenerateContent(
		Template template,
		TfDataTable data);

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		ITemplatesService templateService);

	public string OnCreate(
		string settingsJson,
		ITemplatesService templateService);

	public string OnUpdate(
		Guid templateId,
		string newSettingsJson,
		ITemplatesService templateService);

	public void OnDelete(
		Guid templateId,
		ITemplatesService templateService);


}