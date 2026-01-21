using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.IO.Compression;
using System.Text;
using System.Text.Unicode;
using WebVella.DocumentTemplates.Engines.Html;
using WebVella.DocumentTemplates.Engines.Text;
using WebVella.DocumentTemplates.Engines.TextFile;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Addons;

public class TextFileTemplateProcessor : ITfTemplateProcessorAddon
{
	public const string ID = "1f1139b2-a92c-4ae3-9dc9-318404ab6b04";
	public const string NAME = "Text file template";
	public const string DESCRIPTION = "creates excel files from excel template and data";
	public const string FLUENT_ICON_NAME = "DocumentBulletList";
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
		List<Guid> tfRecordIds,
		List<Guid> tfDatasetIds,
		List<Guid> tfSpaceIds,			
		Guid userId,
		IServiceProvider serviceProvider)
	{
		var result = (TextFileTemplateResult)GenerateResultInternal(
			template, dataTable,tfRecordIds, tfDatasetIds, tfSpaceIds, serviceProvider);

		return new TextFileTemplatePreviewResult
		{
			Errors = result.Errors,
			Items = result.Items
		};
	}

	public ITfTemplateResult ProcessTemplate(
		TfTemplate template,
		TfDataTable dataTable,
		List<Guid> tfRecordIds,
		List<Guid> tfDatasetIds,
		List<Guid> tfSpaceIds,				
		Guid userId,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
		return GenerateResultInternal(template, dataTable, tfRecordIds,tfDatasetIds,tfSpaceIds,serviceProvider);
	}

	private ITfTemplateResult GenerateResultInternal(
	TfTemplate template,
	TfDataTable dataTable,
	List<Guid> tfRecordIds,
	List<Guid> tfDatasetIds,
	List<Guid> tfSpaceIds,		
	IServiceProvider serviceProvider)
	{
		var result = new TextFileTemplateResult();

		var tfService = serviceProvider.GetService<ITfService>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<TextFileTemplateSettings>(template.SettingsJson);

		if (settings.TemplateFileBlobId is null)
		{
			result.Errors.Add(new ValidationError("TemplateFileBlobId", "Template file is not uploaded."));
			return result;
		}

		try
		{
			var bytes = tfService.GetBlobByteArray(settings.TemplateFileBlobId.Value);

			string content = Encoding.UTF8.GetString(bytes);

			string filename = settings.FileName;

			var ext = Path.GetExtension(filename);

			int filesCounter = 0;

			if (ext.ToLowerInvariant() == ".html" || ext.ToLowerInvariant() == ".htm")
			{
				WvHtmlTemplate tmpl = new WvHtmlTemplate
				{
					Template = UTF8Encoding.UTF8.GetString(bytes),
					GroupDataByColumns = settings.GroupBy
				};

				var tmplResult = tmpl.Process(dataTable.ToDataTable());

				if (tmplResult.ResultItems.Count > 1)
				{
					var name = Path.GetFileNameWithoutExtension(filename);
					filename = $"{filename}_{filesCounter}{ext}";
				}

				foreach (var resultItem in tmplResult.ResultItems)
				{
					if (resultItem is null)
						continue;

					try
					{
						var item = new TextFileTemplateResultItem
						{
							FileName = filename,
							NumberOfRows = (int)resultItem.DataTable?.Rows.Count
						};

						if (resultItem.Result is not null)
						{
							item.BlobId = tfService.CreateBlob(resultItem.Result, temporary: true);
							item.DownloadUrl = $"/fs/blob/{item.BlobId}/{filename}";
						}

						foreach (var ctx in resultItem.Contexts)
						{
							item.Errors.AddRange(ctx.Errors.Select(x => new ValidationError("", x)));
						}

						result.Items.Add(item);
					}
					catch (Exception ex)
					{
						result.Items.Add(new TextFileTemplateResultItem
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
			}
			else
			{
				WvTextFileTemplate tmpl = new WvTextFileTemplate
				{
					Template = new MemoryStream(bytes),
					GroupDataByColumns = settings.GroupBy
				};

				var tmplResult = tmpl.Process(dataTable.ToDataTable());

				if (tmplResult.ResultItems.Count > 1)
				{
					var name = Path.GetFileNameWithoutExtension(filename);
					filename = $"{filename}_{filesCounter}{ext}";
				}

				foreach (var resultItem in tmplResult.ResultItems)
				{
					if (resultItem is null)
						continue;

					try
					{
						var item = new TextFileTemplateResultItem
						{
							FileName = filename,
							NumberOfRows = (int)resultItem.DataTable?.Rows.Count
						};

						if (resultItem.Result is not null)
						{
							item.BlobId = tfService.CreateBlob(resultItem.Result, temporary: true);
							item.DownloadUrl = $"/fs/blob/{item.BlobId}/{filename}";
						}

						foreach (var ctx in resultItem.Contexts)
						{
							item.Errors.AddRange(ctx.Errors.Select(x => new ValidationError("", x)));
						}

						result.Items.Add(item);
					}
					catch (Exception ex)
					{
						result.Items.Add(new TextFileTemplateResultItem
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
		TextFileTemplateResult result,
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

		var settings = JsonSerializer.Deserialize<TextFileTemplateSettings>(settingsJson);

		if (string.IsNullOrWhiteSpace(settings.FileName))
		{
			result.Add(new ValidationError(nameof(settings.FileName), "Filename is required"));
		}
		else if (settings.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
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

		var settings = JsonSerializer.Deserialize<TextFileTemplateSettings>(template.SettingsJson);

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
			if (!string.IsNullOrWhiteSpace(existingTemplate.SettingsJson))
			{
				var oldSettings = JsonSerializer.Deserialize<TextFileTemplateSettings>(existingTemplate.SettingsJson);
				blobId = oldSettings.TemplateFileBlobId;
			}
		}

		if (template is not null)
		{
			if (!string.IsNullOrWhiteSpace(template.SettingsJson))
			{
				var newSettings = JsonSerializer.Deserialize<TextFileTemplateSettings>(template.SettingsJson);
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
