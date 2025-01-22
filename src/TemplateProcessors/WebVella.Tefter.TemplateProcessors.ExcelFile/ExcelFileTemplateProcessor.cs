using ClosedXML.Excel;
using System.Text;
using System.IO.Compression;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile;

public class ExcelFileTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.EXCEL_FILE_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter Excel file template";
	public string Description => "creates excel files from excel template and data";
	public string FluentIconName => "DocumentData";
	public TfTemplateResultType ResultType => TfTemplateResultType.File;
	public Type SettingsComponentType => typeof(SettingsComponent);
	public Type ResultPreviewComponentType => typeof(ResultPreviewComponent);
	public Type ResultComponentType => typeof(ResultComponent);
	public Type HelpComponentType => typeof(HelpComponent);

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

		var blobManager = serviceProvider.GetService<ITfBlobManager>();
		var dataManager = serviceProvider.GetService<ITfDataManager>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);

		if (settings.TemplateFileBlobId is null )
		{
			result.Errors.Add(new ValidationError("TemplateFileBlobId", "Template file is not uploaded."));
			return result;
		}

		var groupedData = GroupDataTable(settings.GroupBy, dataTable);

		int filesCounter = 0;
		foreach (var key in groupedData.Keys)
		{
			string filename = settings.FileName;

			filesCounter++;

			if (groupedData.Keys.Count > 1)
			{
				var ext = Path.GetExtension(filename);
				var name = Path.GetFileNameWithoutExtension(filename);
				filename = $"{filename}_{filesCounter}{ext}";
			}

			try
			{
				using var blobStream = blobManager.GetBlobStream(settings.TemplateFileBlobId.Value).Value;

				var excelResult = new TfExcelTemplateProcessResult();

				excelResult.TemplateWorkbook = new XLWorkbook(blobStream);

				excelResult.ProcessExcelTemplate(groupedData[key]);

				using var resultStream = new MemoryStream();

				excelResult.ResultWorkbook.SaveAs(resultStream);

				var resultBlobId = blobManager.CreateBlob(resultStream, temporary: true).Value;

				blobStream.Close();
				resultStream.Close();

				result.Items.Add(new ExcelFileTemplateResultItem
				{
					FileName = filename,
					BlobId = resultBlobId,
					DownloadUrl = $"/fs/blob/{resultBlobId}/{filename}",
					NumberOfRows = groupedData[key].Rows.Count
				});
			}
			catch (Exception ex)
			{
				result.Items.Add(new ExcelFileTemplateResultItem
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
		ExcelFileTemplateResult result,
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
				var fileBytes = blobManager.GetBlobByteArray(item.BlobId.Value, temporary: true).Value;
				var zipArchiveEntry = archive.CreateEntry(item.FileName, CompressionLevel.Fastest);
				using var zipStream = zipArchiveEntry.Open();
				zipStream.Write(fileBytes, 0, fileBytes.Length);
				zipStream.Close();
			}
		}

		var name = Path.GetFileNameWithoutExtension(filename);

		var zipBlobId = blobManager.CreateBlob(zipMs, temporary: true).Value;
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
			var blobManager = serviceProvider.GetService<ITfBlobManager>();
			if (!blobManager.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: false).Value &&
				!blobManager.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: true).Value)
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

		var blobManager = serviceProvider.GetService<ITfBlobManager>();

		var isTmpBlob = blobManager.ExistsBlob(settings.TemplateFileBlobId.Value, temporary: true).Value;
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

		var existingTemplate = templateServise.GetTemplate(template.Id).Value;

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
						blobManager.DeleteBlob(blobId.Value);
					}

					if (newSettings.TemplateFileBlobId is not null)
					{
						//make new blob persistent
						var isTmpBlob = blobManager.ExistsBlob(newSettings.TemplateFileBlobId.Value, temporary: true).Value;
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
