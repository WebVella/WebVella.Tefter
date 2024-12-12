namespace WebVella.Tefter.Templates.ContentProcessors;

public class TfFileContentProcessor : ITemplateProcessor
{
	public string Name => "Default tefter file content processor";

	public string Description => "Default file email content processor";

	public TemplateResultType ResultType => TemplateResultType.File;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;

	public ITemplateResult GenerateContent(
		Template template, 
		TfDataTable data)
	{
		return null;
	}

	public List<string> GetUsedColumns(
		string settingsJson, 
		ITemplatesService templateService)
	{
		var settings = JsonSerializer.Deserialize<TemplateFileSettings>(settingsJson);

		List<string> usedColumns = new List<string>();

		if (!string.IsNullOrWhiteSpace(settings.GroupBy))
			usedColumns.Add(settings.GroupBy);

		var tags = TfTemplateUtility.GetTagsFromTemplate(settings.FileName ?? string.Empty);

		foreach (var tag in tags)
		{
			if (tag.Type == TfTemplateTagType.Data)
			{
				if (!usedColumns.Contains(tag.Name))
					usedColumns.Add(tag.Name);
			}
		}

		return usedColumns;
	}

	public List<Template> GetUsedTemplates(
		string settingsJson, 
		ITemplatesService templateService)
	{
		return new List<Template>();
	}
}
