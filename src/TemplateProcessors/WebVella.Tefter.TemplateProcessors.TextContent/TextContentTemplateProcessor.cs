using System.Text;
using WebVella.Tefter.TemplateProcessors.TextContent.Components;

namespace WebVella.Tefter.TemplateProcessors.TextContent;

public class TextContentTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.TEXT_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter text content template";
	public string Description => "creates text content from a template and data";
	public string FluentIconName => "ScanText";
	public TfTemplateResultType ResultType => TfTemplateResultType.Text;
	public Type SettingsComponentType => typeof(ManageSettingsComponent);
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

		var blobManager = serviceProvider.GetService<ITfBlobManager>();
		var dataManager = serviceProvider.GetService<ITfDataManager>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<TextContentTemplateSettings>(template.SettingsJson);

		result.IsHtml = settings.IsHtml;

		var groupedData = GroupDataTable(settings.GroupBy, dataTable);

		foreach (var key in groupedData.Keys)
		{
			var item = new TextContentTemplateResultItem();

			try
			{
				if (settings.IsHtml)
				{
					var htmlProcessResult = new TfHtmlTemplateProcessResult();
					htmlProcessResult.TemplateHtml = settings.Content ?? string.Empty;
					htmlProcessResult.ProcessHtmlTemplate(groupedData[key]);
					item.Content = htmlProcessResult.ResultHtml ?? string.Empty;
				}
				else
				{
					var textProcessResult = new TfTextTemplateProcessResult();
					textProcessResult.TemplateText = settings.Content ?? string.Empty;
					textProcessResult.ProcessTextTemplate(groupedData[key]);
					item.Content = textProcessResult.ResultText ?? string.Empty;
				}

				item.NumberOfRows = (int)groupedData[key].Rows.Count;

				result.Items.Add(item);
			}
			catch (Exception ex)
			{
				item.Errors.Add(new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}"));
			}

		}

		return result;
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
