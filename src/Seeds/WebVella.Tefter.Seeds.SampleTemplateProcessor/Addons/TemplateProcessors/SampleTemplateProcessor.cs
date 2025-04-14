using System.Text;

namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Addons;

public class SampleTemplateProcessor : ITfTemplateProcessorAddon
{

	public const string ID = "9754c7dc-80da-4b65-bf10-eb441d370fe2";
	public const string NAME = "Sample content template";
	public const string DESCRIPTION = "scrambles text";
	public const string FLUENT_ICON_NAME = "ScanText";
	public const TfTemplateResultType RESULT_TYPE = TfTemplateResultType.Text;

	public Guid Id  { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public TfTemplateResultType ResultType  { get; init; } = RESULT_TYPE;

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
		var result = (SampleTemplateResult)GenerateResultInternal(
			template, dataTable, serviceProvider);

		return new SampleTemplatePreviewResult
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
		var result = new SampleTemplateResult();


		var tfService = serviceProvider.GetService<ITfService>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<SampleTemplateSettings>(template.SettingsJson);
	
		try
		{

			foreach (TfDataRow row in dataTable.Rows)
			{
				var item = new SampleTemplateResultItem();
				item.NumberOfRows = 1;

				//foreach row scrables values of TEXT type and append them to string builder
				StringBuilder sb = new StringBuilder();
				foreach (var column in dataTable.Columns)
				{
					if (column.DbType == TfDatabaseColumnType.Text)
					{
						if (row[column.Name] == null)
							continue;

						var scrabledText = Scramble( row[column.Name].ToString());
						sb.Append(scrabledText);
					}
				}
				item.Content = sb.ToString();
				result.Items.Add(item);
			}
		}
		catch (Exception ex)
		{
			result.Errors.Add(new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}"));
		}

		return result;
	}

	private static Random rand = new Random();

	private string Scramble( String str)
	{
		var list = new SortedList<int, char>();
		foreach (var c in str)
			list.Add(rand.Next(), c);
		return new string(list.Values.ToArray());
	}
}
