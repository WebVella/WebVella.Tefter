using ClosedXML.Excel;
using System.IO.Compression;
using System.Text;
using WebVella.Tefter.TemplateProcessors.TextFile.Components;

namespace WebVella.Tefter.TemplateProcessors.TextFile;

public class TextFileTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.TEXT_FILE_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter text file template";
	public string Description => "creates excel files from excel template and data";
	public string FluentIconName => "DocumentBulletList";
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
		var result = (TextFileTemplateResult)GenerateResultInternal(
			template, dataTable, serviceProvider);

		return new TextFileTemplatePreviewResult
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
		var result = new TextFileTemplateResult();

		var blobManager = serviceProvider.GetService<ITfBlobManager>();
		var dataManager = serviceProvider.GetService<ITfDataManager>();

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

		var groupedData = GroupDataTable(settings.GroupBy, dataTable);

		var bytes = blobManager.GetBlobByteArray(settings.TemplateFileBlobId.Value);

		string content = Encoding.UTF8.GetString(bytes);

		int filesCounter = 0;
		foreach (var key in groupedData.Keys)
		{
			string filename = settings.FileName;

			filesCounter++;

			var ext = Path.GetExtension(filename);

			if (groupedData.Keys.Count > 1)
			{
				var name = Path.GetFileNameWithoutExtension(filename);
				filename = $"{filename}_{filesCounter}{ext}";
			}

			try
			{
				string processedContent = string.Empty;

				if(ext.ToLowerInvariant() == ".html" || ext.ToLowerInvariant() == ".htm")
				{
					var htmlProcessResult = new TfHtmlTemplateProcessResult();
					htmlProcessResult.TemplateHtml = content ?? string.Empty;
					htmlProcessResult.ProcessHtmlTemplate(groupedData[key]);
					processedContent = htmlProcessResult.ResultHtml;
				}
				else
				{
					var textProcessResult = new TfTextTemplateProcessResult();
					textProcessResult.TemplateText = content ?? string.Empty;
					textProcessResult.ProcessTextTemplate(groupedData[key]);
					processedContent = textProcessResult.ResultText;
				}

				using var resultStream = new MemoryStream();

				bytes = UTF8Encoding.UTF8.GetBytes(processedContent);

				resultStream.Write(bytes, 0, bytes.Length);

				var resultBlobId = blobManager.CreateBlob(resultStream, temporary: true);

				resultStream.Close();

				result.Items.Add(new TextFileTemplateResultItem
				{
					FileName = filename,
					BlobId = resultBlobId,
					DownloadUrl = $"/fs/blob/{resultBlobId}/{filename}",
					NumberOfRows = groupedData[key].Rows.Count
				});
			}
			catch (Exception ex)
			{
				result.Items.Add(new TextFileTemplateResultItem
				{
					FileName = filename,
					DownloadUrl = null,
					BlobId = null,
					NumberOfRows = groupedData[key].Rows.Count,
					Errors = new()
					{
						new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}")
					}
				});
			}
		}

		GenerateZipFile(settings.FileName, result, blobManager);

		return result;
	}

	private void GenerateZipFile(
		string filename,
		TextFileTemplateResult result,
		ITfBlobManager blobManager)
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
				var fileBytes = blobManager.GetBlobByteArray(item.BlobId.Value, temporary: true);
				var zipArchiveEntry = archive.CreateEntry(item.FileName, CompressionLevel.Fastest);
				using var zipStream = zipArchiveEntry.Open();
				zipStream.Write(fileBytes, 0, fileBytes.Length);
				zipStream.Close();
			}
		}

		var name = Path.GetFileNameWithoutExtension(filename);

		var zipBlobId = blobManager.CreateBlob(zipMs, temporary: true);
		result.ZipFilename = $"{name}.zip";
		result.ZipBlobId = zipBlobId;
		result.ZipDownloadUrl = $"/fs/blob/{zipBlobId}/{name}.zip";
	}

	private Dictionary<string, TfDataTable> GroupDataTable(
		List<string> groupColumns,
		TfDataTable dataTable)
	{
		var result = new Dictionary<string, TfDataTable>();
		if (groupColumns is null || groupColumns.Count == 0)
		{
			result.Add(string.Empty, dataTable);
		}
		else
		{
			foreach (TfDataRow row in dataTable.Rows)
			{
				var sbKey = new StringBuilder();

				foreach (var column in groupColumns)
				{
					sbKey.Append($"{row[column]}$$$|||$$$");
				}

				var key = sbKey.ToString();

				if (!result.ContainsKey(key))
				{
					result.Add(key, dataTable.NewTable());
				}

				result[key].Rows.Add(row);
			}
		}

		return result;
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
			var blobManager = serviceProvider.GetService<ITfBlobManager>();
			if (!blobManager.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: false) &&
				!blobManager.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: true))
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

		var blobManager = serviceProvider.GetService<ITfBlobManager>();

		var isTmpBlob = blobManager.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: true);
		if (isTmpBlob)
		{
			blobManager.MakeTempBlobPermanent(settings.TemplateFileBlobId.Value);
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

		var existingTemplate = templateServise.GetTemplate(template.Id);

		if (existingTemplate is not null)
		{
			if (string.IsNullOrWhiteSpace(existingTemplate.SettingsJson))
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
						blobManager.DeleteBlob(blobId.Value);
					}

					if (newSettings.TemplateFileBlobId is not null)
					{
						//make new blob persistent
						var isTmpBlob = blobManager.ExistsBlob(newSettings.TemplateFileBlobId.Value, temporary: true);
						if (isTmpBlob)
						{
							blobManager.MakeTempBlobPermanent(newSettings.TemplateFileBlobId.Value);
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
