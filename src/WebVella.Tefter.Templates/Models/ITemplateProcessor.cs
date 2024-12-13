namespace WebVella.Tefter.Templates.Models;

public interface ITemplateProcessor
{
	public string Name { get; }
	public string Description { get; }
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

	public string OnCreateTemplate(
		string settingsJson,
		ITemplatesService templateService);

	public string OnUpdateTemplate(
		Guid templateId,
		string newSettingsJson,
		ITemplatesService templateService);

	public void OnDeleteTemplate(
		Guid templateId,
		ITemplatesService templateService);


}