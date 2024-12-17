namespace WebVella.Tefter.TemplateProcessors.ExcelFile;

using TfTemplate = WebVella.Tefter.Models.TfTemplate;

public class ExcelFileTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.EXCEL_FILE_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter Excel file template";

	public string Description => "creates excel files from excel template and data";

	public string FluentIconName => "Table";

	public TfTemplateResultType ResultType => TfTemplateResultType.File;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template, 
		TfDataTable data)
	{
		return null;
	}

	public List<string> GetUsedColumns(
		string settingsJson,
		ITfTemplateService templateService)
	{
		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(settingsJson);

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

	public List<TfTemplate> GetUsedTemplates(
		string settingsJson,
		ITfTemplateService templateService)
	{
		//TODO
		return new List<TfTemplate>();
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		ITfTemplateService templateService)
	{
		//TODO
		throw new NotImplementedException();
	}

	public string OnCreate(
		string settingsJson,
		ITfTemplateService templateService)
	{
		//TODO
		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(settingsJson);
		return settingsJson;
	}

	public string OnUpdate(
		Guid templateId,
		string newSettingsJson,
		ITfTemplateService templateService)
	{
		//TODO
		return newSettingsJson;

	}

	public void OnDelete(
		Guid templateId,
		ITfTemplateService templateService)
	{
		//TODO
	}
}
