using System.Diagnostics;
using WebVella.Tefter.TemplateProcessors.TextFile.Components;

namespace WebVella.Tefter.TemplateProcessors.TextFile;

public class TextFileTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.TEXT_FILE_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter text file template";

	public string Description => "creates excel files from excel template and data";

	public string FluentIconName => "DocumentBulletList";

	public TfTemplateResultType ResultType => TfTemplateResultType.File;

	public Type SettingsComponentType => typeof(SettingsComponent);

	public Type ResultViewComponentType => typeof(ResultViewComponent);
	public Type HelpComponentType => typeof(HelpComponent);

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return null;
	}

	public List<ValidationError> ProcessTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
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
		//TODO
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
		//TODO
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
