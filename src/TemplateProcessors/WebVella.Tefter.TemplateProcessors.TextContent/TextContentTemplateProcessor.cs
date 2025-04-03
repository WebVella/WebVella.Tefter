using System.Text;
using WebVella.DocumentTemplates.Engines.Html;
using WebVella.DocumentTemplates.Engines.Text;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.TemplateProcessors.TextContent.Components;

namespace WebVella.Tefter.TemplateProcessors.TextContent;

public class TextContentTemplateProcessor : ITfTemplateProcessor
{
	/// <summary>
	/// used as unique identifier
	/// </summary>
	public Guid Id => Constants.TEXT_CONTENT_PROCESSOR_ID;
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string Name => "Text content template";
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string Description => "creates text content from a template and data";
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string FluentIconName => "ScanText";
	/// <summary>
	/// expected result type. used to group processors in the Admin panel list.
	/// </summary>
	public TfTemplateResultType ResultType => TfTemplateResultType.Text;

	/// <summary>
	/// Called when template preview result is submitted for generation.
	/// Sometimes when the user selects template from this processor, 
	/// it needs to provide input needed by the processor to generate the final result.
	/// This method validates this input
	/// </summary>
	public void ValidatePreview(
		TfTemplate template,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
	}

	/// <summary>
	/// Presented to the user when it wants to use this processor with data.
	/// </summary>
	/// <returns></returns>
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

	/// <summary>
	/// Method generating the final result
	/// </summary>
	/// <returns></returns>
	public ITfTemplateResult ProcessTemplate(
		TfTemplate template,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
		return GenerateResultInternal(template, dataTable, serviceProvider);
	}

	/// <summary>
	/// Called when user submits settings
	/// </summary>
	public List<ValidationError> ValidateSettings(
			string settingsJson,
			IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	/// <summary>
	/// Called in transaction with the creation method, but before it.
	/// </summary>
	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	/// <summary>
	/// called after the creation method outside its transaction
	/// </summary>
	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	/// <summary>
	/// Called in transaction with the update method, but before it.
	/// </summary>
	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	/// <summary>
	/// called after the update method outside its transaction
	/// </summary>
	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}
	/// <summary>
	/// Called in transaction with the update method, but before it.
	/// </summary>
	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}
	/// <summary>
	/// called after the delete method outside its transaction
	/// </summary>
	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
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
}
