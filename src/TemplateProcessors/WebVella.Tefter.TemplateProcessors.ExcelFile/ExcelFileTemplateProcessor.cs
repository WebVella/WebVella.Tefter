namespace WebVella.Tefter.TemplateProcessors.ExcelFile;

public class ExcelFileTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.EXCEL_FILE_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter Excel file template";

	public string Description => "creates excel files from excel template and data";

	public string FluentIconName => "DocumentData";

	public TfTemplateResultType ResultType => TfTemplateResultType.File;

	public Type SettingsComponentType => typeof(SettingsComponent);

	public Type ResultViewComponentType => null;

	public List<string> GetUsedColumns(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		List<string> usedColumns = new List<string>();

		if (string.IsNullOrEmpty(settingsJson))
		{
			return usedColumns;
		}

		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(settingsJson);

		if (settings.GroupBy != null && settings.GroupBy.Count > 0)
		{
			usedColumns.AddRange(settings.GroupBy);
		}

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
		IServiceProvider serviceProvider)
	{
		return new List<TfTemplate>();
	}

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return null;
	}

	public void ProcessTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{

		if (String.IsNullOrWhiteSpace(template.SettingsJson))
			return new List<ValidationError>();

		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);

		if( settings.BlobId is null )
			return new List<ValidationError>();

		var blobManager = serviceProvider.GetService<ITfBlobManager>();

		var isTmpBlob = blobManager.ExistsBlob(settings.BlobId.Value, temporary: true).Value;
		if (isTmpBlob)
		{
			blobManager.MakeTempBlobPermanent(settings.BlobId.Value);
		}

		return new List<ValidationError>();
	}

	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
	}

	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		var blobManager = serviceProvider.GetService<ITfBlobManager>();
		var templateServise = serviceProvider.GetService<ITfTemplateService>();

		Guid? blobId = null;

		var existingTemplate = templateServise.GetTemplate(template.Id).Value;

		if (existingTemplate is not null)
		{
			if (string.IsNullOrWhiteSpace(existingTemplate.SettingsJson))
			{
				var oldSettings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(existingTemplate.SettingsJson);
				blobId = oldSettings.BlobId;
			}
		}

		if (template is not null)
		{
			if (string.IsNullOrWhiteSpace(template.SettingsJson))
			{
				var newSettings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(existingTemplate.SettingsJson);
				if (newSettings.BlobId != blobId)
				{
					//delete old blob
					if (blobId is not null)
					{
						blobManager.DeleteBlob(blobId.Value);
					}

					if (newSettings.BlobId is not null)
					{
						//make new blob persistent
						var isTmpBlob = blobManager.ExistsBlob(newSettings.BlobId.Value, temporary: true).Value;
						if (isTmpBlob)
						{
							blobManager.MakeTempBlobPermanent(newSettings.BlobId.Value);
						}
					}
				}
			}
		}


		return new List<ValidationError>();
	}

	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
	}

	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		//TODO
	}
}
