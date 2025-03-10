using System.Text;
using WebVella.DocumentTemplates.Engines.Html;
using WebVella.DocumentTemplates.Engines.Text;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.TemplateProcessors.TextContent.Components;

namespace WebVella.Tefter.TemplateProcessors.TextContent;

public class TextContentTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.TEXT_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter text content template";
	public string Description => "creates text content from a template and data";
	public string FluentIconName => "ScanText";
	public TfTemplateResultType ResultType => TfTemplateResultType.Text;

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
		var result = (TextContentTemplateResult)GenerateResultInternal(
			template, dataTable, serviceProvider);

		return new TextContentTemplatePreviewResult
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
		var result = new TextContentTemplateResult();

		var tfService = serviceProvider.GetService<ITfService>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<TextContentTemplateSettings>(template.SettingsJson);

		result.IsHtml = settings.IsHtml;

		DataTable dt = dataTable.ToDataTable();

		try
		{
			if (!settings.IsHtml)
			{
				var wvTemplate = new WvTextTemplate
				{
					GroupDataByColumns = settings.GroupBy,
					Template = settings.Content
				};

				var res = wvTemplate.Process(dt);
				foreach (var resultItem in res.ResultItems)
				{
					var item = new TextContentTemplateResultItem();
					item.Content = resultItem.Result;
					item.NumberOfRows = (int)resultItem.DataTable?.Rows.Count;
					foreach (var ctx in resultItem.Contexts)
					{
						item.Errors.AddRange(ctx.Errors.Select(x => new ValidationError("", x)));
					}
					result.Items.Add(item);
				}
			}
			else
			{
				var wvTemplate = new WvHtmlTemplate
				{
					GroupDataByColumns = settings.GroupBy,
					Template = settings.Content
				};

				var res = wvTemplate.Process(dt);
				foreach (var resultItem in res.ResultItems)
				{
					var item = new TextContentTemplateResultItem();
					item.Content = resultItem.Result;
					item.NumberOfRows = (int)resultItem.DataTable?.Rows.Count;
					foreach (var ctx in resultItem.Contexts)
					{
						item.Errors.AddRange(ctx.Errors.Select(x => new ValidationError("", x)));
					}
					result.Items.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			result.Errors.Add(new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}"));
		}

		return result;
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}
}
