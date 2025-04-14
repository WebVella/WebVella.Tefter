using System.IO.Compression;
using WebVella.DocumentTemplates.Engines.SpreadsheetFile;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Addons;

public class ExcelFileTemplateProcessor : ITfTemplateProcessor
{
	public const string ID = "a655fffd-1a02-4e90-9e05-50595916f97a";
	public const string NAME = "Excel file template";
	public const string DESCRIPTION = "creates excel files from excel template and data";
	public const string FLUENT_ICON_NAME = "DocumentData";
	public const TfTemplateResultType RESULT_TYPE = TfTemplateResultType.File;

	public Guid Id  { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public TfTemplateResultType ResultType  { get; init; } = RESULT_TYPE;



	public void ValidatePreview(
		TfTemplate template,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
	}
	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		TfTemplate template,
		TfDataTable dataTable,
		IServiceProvider serviceProvider)
	{
		var result = (ExcelFileTemplateResult)GenerateResultInternal(
			template, dataTable, serviceProvider);

		return new ExcelFileTemplatePreviewResult
		{
			Errors = result.Errors,
			Items = result.Items
		};
	}

	public ITfTemplateResult ProcessTemplate(
		TfTemplate template,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
		return GenerateResultInternal(template, dataTable, serviceProvider);
	}

	private ITfTemplateResult GenerateResultInternal(
		TfTemplate template,
		TfDataTable dataTable,
		IServiceProvider serviceProvider)
	{
		var result = new ExcelFileTemplateResult();

		var tfService = serviceProvider.GetService<ITfService>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);

		if (settings.TemplateFileBlobId is null)
		{
			result.Errors.Add(new ValidationError("TemplateFileBlobId", "Template file is not uploaded."));
			return result;
		}

		try
		{
			var bytes = tfService.GetBlobByteArray(settings.TemplateFileBlobId.Value);

			WvSpreadsheetFileTemplate tmpl = new WvSpreadsheetFileTemplate
			{
				Template = new MemoryStream(bytes),
				GroupDataByColumns = settings.GroupBy
			};

			var tmplResult = tmpl.Process(dataTable.ToDataTable());

			int filesCounter = 0;

			foreach (var resultItem in tmplResult.ResultItems)
			{
				if (resultItem is null)
					continue;

				string filename = settings.FileName;

				filesCounter++;

				if (tmplResult.ResultItems.Count > 1)
				{
					var ext = Path.GetExtension(filename);
					var name = Path.GetFileNameWithoutExtension(filename);
					filename = $"{filename}_{filesCounter}{ext}";
				}

				try
				{
					var item = new ExcelFileTemplateResultItem
					{
						FileName = filename,
						NumberOfRows = (int)resultItem.DataTable?.Rows.Count
					};

					if (resultItem.Result is not null)
					{
						item.BlobId = tfService.CreateBlob(resultItem.Result, temporary: true);
						item.DownloadUrl = $"/fs/blob/{item.BlobId}/{filename}";
					}

					result.Items.Add(item);
				}
				catch (Exception ex)
				{
					result.Items.Add(new ExcelFileTemplateResultItem
					{
						FileName = filename,
						DownloadUrl = null,
						BlobId = null,
						NumberOfRows = (int)resultItem.DataTable?.Rows.Count,
						Errors = new()
						{
							new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}")
						}
					});
				}
			}

			GenerateZipFile(settings.FileName, result, tfService);
		}
		catch (Exception ex)
		{
			result.Errors.Add(new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}"));
		}

		return result;
	}

	private void GenerateZipFile(
		string filename,
		ExcelFileTemplateResult result,
		ITfService tfService)
	{
		var validItems = result.Items
			.Where(x => x.Errors.Count == 0 && x.BlobId.HasValue)
			.ToList();

		if (validItems.Count == 0)
		{
			return;
		}

		using MemoryStream zipMs = new MemoryStream();

		using (var archive = new ZipArchive(zipMs, ZipArchiveMode.Create, true))
		{
			foreach (var item in validItems)
			{
				var fileBytes = tfService.GetBlobByteArray(item.BlobId.Value, temporary: true);
				var zipArchiveEntry = archive.CreateEntry(item.FileName, CompressionLevel.Fastest);
				using var zipStream = zipArchiveEntry.Open();
				zipStream.Write(fileBytes, 0, fileBytes.Length);
				zipStream.Close();
			}
		}

		var name = Path.GetFileNameWithoutExtension(filename);

		var zipBlobId = tfService.CreateBlob(zipMs, temporary: true);
		result.ZipFilename = $"{name}.zip";
		result.ZipBlobId = zipBlobId;
		result.ZipDownloadUrl = $"/fs/blob/{zipBlobId}/{name}.zip";
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		var result = new List<ValidationError>();

		if (string.IsNullOrWhiteSpace(settingsJson))
		{
			return result;
		}

		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(settingsJson);

		if (string.IsNullOrWhiteSpace(settings.FileName))
		{
			result.Add(new ValidationError(nameof(settings.FileName), "Filename is required"));
		}
		else if (settings.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) != 0)
		{
			result.Add(new ValidationError(nameof(settings.FileName), "Filename is invalid"));
		}

		if (settings.TemplateFileBlobId is null)
		{
			result.Add(new ValidationError(nameof(settings.TemplateFileBlobId), "Template file is not specified"));
		}
		else
		{
			var tfService = serviceProvider.GetService<ITfService>();
			if (!tfService.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: false) &&
				!tfService.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: true))
			{
				result.Add(new ValidationError(nameof(settings.TemplateFileBlobId), "Template file is not found."));
			}
		}

		return result;
	}

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{

		if (String.IsNullOrWhiteSpace(template.SettingsJson))
			return new List<ValidationError>();

		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);

		if (settings.TemplateFileBlobId is null)
			return new List<ValidationError>();

		var tfService = serviceProvider.GetService<ITfService>();

		var isTmpBlob = tfService.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: true);
		if (isTmpBlob)
		{
			tfService.MakeTempBlobPermanent(settings.TemplateFileBlobId.Value);
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
		var tfService = serviceProvider.GetService<ITfService>();

		Guid? blobId = null;

		var existingTemplate = tfService.GetTemplate(template.Id);

		if (existingTemplate is not null)
		{
			if (string.IsNullOrWhiteSpace(existingTemplate.SettingsJson))
			{
				var oldSettings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(existingTemplate.SettingsJson);
				blobId = oldSettings.TemplateFileBlobId;
			}
		}

		if (template is not null)
		{
			if (!string.IsNullOrWhiteSpace(template.SettingsJson))
			{
				var newSettings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);
				if (newSettings.TemplateFileBlobId != blobId)
				{
					//delete old blob
					if (blobId is not null)
					{
						tfService.DeleteBlob(blobId.Value);
					}

					if (newSettings.TemplateFileBlobId is not null)
					{
						//make new blob persistent
						var isTmpBlob = tfService.ExistsBlob(newSettings.TemplateFileBlobId.Value, temporary: true);
						if (isTmpBlob)
						{
							tfService.MakeTempBlobPermanent(newSettings.TemplateFileBlobId.Value);
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
