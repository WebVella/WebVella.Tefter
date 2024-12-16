namespace WebVella.Tefter.Templates.TemplateProcessors;

public class FileTemplateProcessor : ITemplateProcessor
{
	public Guid Id => TemplatesConstants.FILE_CONTENT_PROCESSOR_ID;
	public string Name => "Default tefter file template processor";

	public string Description => "Default file email template processor";

	public string FluentIconName => "Table";

	public TemplateResultType ResultType => TemplateResultType.File;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;

	public ITemplateResult GenerateTemplateResult(
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

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		ITemplatesService templateService)
	{
		throw new NotImplementedException();
	}

	public string OnCreate(
		string settingsJson,
		ITemplatesService templateService)
	{
		var settings = JsonSerializer.Deserialize<TemplateFileSettings>(settingsJson);
		return settingsJson;
	}

	public string OnUpdate(
		Guid templateId,
		string newSettingsJson,
		ITemplatesService templateService)
	{
		//do nothing
		return newSettingsJson;

	}

	public void OnDelete(
		Guid templateId,
		ITemplatesService templateService)
	{
		//do nothing
	}
}
