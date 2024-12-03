namespace WebVella.Tefter.Templates.Models;

public interface ITemplateProcessor
{
	public string Name { get; }
	public string Description { get; }
	public TemplateResultType ResultType { get; }
	public List<string> GetUsedCollumns(string settingsJson);
	public TemplateResult GenerateContent(string settingsJson, TfDataTable data);
	public Type SettingsComponentType { get; }
	public Type ResultViewComponentType { get; }
}