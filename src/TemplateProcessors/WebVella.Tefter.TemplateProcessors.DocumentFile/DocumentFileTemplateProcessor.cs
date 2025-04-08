using System.IO.Compression;
using WebVella.DocumentTemplates.Engines.DocumentFile;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile;

public class DocumentFileTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.DOCUMENT_FILE_CONTENT_PROCESSOR_ID;
	public string Name => "Word file template";
	public string Description => "creates word files from word template and data";
	public string FluentIconName => "DocumentData";
	public TfTemplateResultType ResultType => TfTemplateResultType.File;

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
		var result = (DocumentFileTemplateResult)GenerateResultInternal(
			template, dataTable, serviceProvider);

		return new DocumentFileTemplatePreviewResult
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
		var result = new DocumentFileTemplateResult();

		var tfService = serviceProvider.GetService<ITfService>();
		if(tfService is null) 
			throw new Exception("tfService not found");

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<DocumentFileTemplateSettings>(template.SettingsJson);

		if (settings is null || settings.TemplateFileBlobId is null)
		{
			result.Errors.Add(new ValidationError("TemplateFileBlobId", "Template file is not uploaded."));
			return result;
		}

		try
		{
			var bytes = tfService.GetBlobByteArray(settings.TemplateFileBlobId.Value);

			WvDocumentFileTemplate tmpl = new WvDocumentFileTemplate
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

				string filename = settings.FileName ?? String.Empty;

				filesCounter++;

				if (tmplResult.ResultItems.Count > 1)
				{
					var ext = Path.GetExtension(filename);
					var name = Path.GetFileNameWithoutExtension(filename);
					filename = $"{filename}_{filesCounter}{ext}";
				}

				try
				{
					var item = new DocumentFileTemplateResultItem
					{
						FileName = filename,
						NumberOfRows = (int)(resultItem.DataTable?.Rows.Count ?? 0)
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
					result.Items.Add(new DocumentFileTemplateResultItem
					{
						FileName = filename,
						DownloadUrl = String.Empty,
						BlobId = null,
						NumberOfRows = (int)(resultItem.DataTable?.Rows.Count ?? 0),
						Errors = new()
						{
							new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}")
						}
					});
				}
			}
			if (!String.IsNullOrEmpty(settings.FileName))
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
		DocumentFileTemplateResult result,
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
				if (!item.BlobId.HasValue) continue;
				if (String.IsNullOrWhiteSpace(item.FileName)) continue;
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

		var settings = JsonSerializer.Deserialize<DocumentFileTemplateSettings>(settingsJson);

		if (settings is null || string.IsNullOrWhiteSpace(settings.FileName))
		{
			result.Add(new ValidationError(nameof(settings.FileName), "Filename is required"));
		}
		else if (settings.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) != 0)
		{
			result.Add(new ValidationError(nameof(settings.FileName), "Filename is invalid"));
		}

		if (settings is null || settings.TemplateFileBlobId is null)
		{
			result.Add(new ValidationError(nameof(settings.TemplateFileBlobId), "Template file is not specified"));
		}
		else
		{
			var tfService = serviceProvider.GetService<ITfService>();
			if(tfService is null) throw new Exception("tfService not found");

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

		var settings = JsonSerializer.Deserialize<DocumentFileTemplateSettings>(template.SettingsJson);

		if (settings is null || settings.TemplateFileBlobId is null)
			return new List<ValidationError>();

		var tfService = serviceProvider.GetService<ITfService>();
		if(tfService is null) throw new Exception("tfService not found");
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
		if(tfService is null) throw new Exception("tfService not found");
		Guid? blobId = null;

		var existingTemplate = tfService.GetTemplate(template.Id);

		if (existingTemplate is not null)
		{
			if (string.IsNullOrWhiteSpace(existingTemplate.SettingsJson))
			{
				var oldSettings = JsonSerializer.Deserialize<DocumentFileTemplateSettings>(existingTemplate.SettingsJson);
				blobId = oldSettings?.TemplateFileBlobId;
			}
		}

		if (template is not null)
		{
			if (!string.IsNullOrWhiteSpace(template.SettingsJson))
			{
				var newSettings = JsonSerializer.Deserialize<DocumentFileTemplateSettings>(template.SettingsJson);
				if (newSettings?.TemplateFileBlobId != blobId)
				{
					//delete old blob
					if (blobId is not null)
					{
						tfService.DeleteBlob(blobId.Value);
					}

					if (newSettings?.TemplateFileBlobId is not null)
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
