namespace WebVella.Tefter.UI.Addons;

public class TfImageViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>

	public const string ID = "e4ac0380-7285-40a2-a4ba-4a7b28e4a345";
	public const string NAME = "Image";
	public const string DESCRIPTION = "displays an Image";
	public const string FLUENT_ICON_NAME = "Image";
	private const string VALUE_ALIAS = "Value";

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;

	public List<TfSpaceViewColumnDataMappingDefinition> DataMappingDefinitions { get; init; } = new()
	{
		new TfSpaceViewColumnDataMappingDefinition
		{
			Alias = VALUE_ALIAS,
			Description =
				"this column is compatible with all column types, but its intended use is with text",
			SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType>
			{
				TfDatabaseColumnType.ShortText, TfDatabaseColumnType.Text
			}
		}
	};

	#endregion

	#region << PRIVATE >>

	private List<ValidationError> _validationErrors = new();
	#endregion
	
	#region << PUBLIC >>

	public void ProcessExcelCell(TfSpaceViewColumnBaseContext args)
	{
		if (args is not TfSpaceViewColumnExportExcelModeContext)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportExcelModeContext is expected");
		if (args is TfSpaceViewColumnExportExcelModeContext context)
			context.ExcelCell.SetValue(XLCellValue.FromObject(String.Join(", ", _initValue(args))));
	}

	//Returns Value/s as string usually for CSV export
	public string GetValueAsString(TfSpaceViewColumnBaseContext args)
	{
		if (args is not TfSpaceViewColumnExportCsvModeContext)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportCsvModeContext is expected");

		return String.Join(", ", _initValue(args));
	}

	public RenderFragment Render(TfSpaceViewColumnBaseContext args)
	{
		if (args is null)
			throw new Exception("TfSpaceViewColumnBaseContext is expected");
		if (args is TfSpaceViewColumnReadModeContext readContext)
			return _renderReadMode(readContext);
		if (args is TfSpaceViewColumnEditModeContext editContext)
			return _renderEditMode(editContext);
		if (args is TfSpaceViewColumnOptionsModeContext optionsContext)
			return _renderOptionsMode(optionsContext);

		throw new Exception("Unsupported render mode");
	}

	public List<ValidationError> ValidateTypeOptions(TfSpaceViewColumnOptionsModeContext args)
	{
		_validationErrors = new();
		return _validationErrors;
	}

	#endregion

	#region << Private >>

	//Value
	private List<string?> _initValue(TfSpaceViewColumnBaseContext args)
	{
		var values = new List<string?>();

		var (column, columnData) = args.GetColumnAndDataByAlias(VALUE_ALIAS);
		if (columnData is null)
		{
			values.Add(String.Empty);
		}
		else if (column.Origin == TfDataColumnOriginType.JoinedProviderColumn)
		{
			if (columnData.GetType().ImplementsInterface(typeof(IList)))
			{
				if (columnData is IList joinValues)
				{
					foreach (var joinValue in joinValues)
						values.Add(joinValue?.ToString());
				}
			}
		}
		else
			values.Add(columnData.ToString());

		return values;
	}

	//Render
	private RenderFragment _renderReadMode(TfSpaceViewColumnReadModeContext context)
	{
		var values = _initValue(context);
		return builder =>
		{
			builder.OpenComponent<TucImageViewColumnTypeRead>(0);
			builder.AddAttribute(1, "Value", values);
			builder.AddAttribute(2, "Settings", context.GetColumnTypeOptions(new TfImageViewColumnTypeSettings()));
			builder.CloseComponent();
		};
	}

	private RenderFragment _renderEditMode(TfSpaceViewColumnEditModeContext context)
	{
		var (column, _) = context.GetColumnAndDataByAlias(VALUE_ALIAS);
		//Editable columns
		if (column.Origin == TfDataColumnOriginType.CurrentProvider
		    || column.Origin == TfDataColumnOriginType.SharedColumn)
		{
			var values = _initValue(context);

			return builder =>
			{
				builder.OpenComponent<TucImageViewColumnTypeEdit>(0);
				builder.AddAttribute(1, "Value", values[0]);
				builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, async (x) =>
				{
					var change = new TfSpaceViewColumnDataChange
					{
						RowId = context.RowId, DataChange = new() { { column.Name, x } }
					};
					await context.DataChanged.InvokeAsync(change);
				}));
				builder.AddAttribute(3, "Settings", context.GetColumnTypeOptions(new TfImageViewColumnTypeSettings()));
				builder.CloseComponent();
			};
		}

		//Non Editable columns
		return _renderReadMode(new TfSpaceViewColumnReadModeContext(context.ViewData)
		{
			TfService = context.TfService,
			ViewColumn = context.ViewColumn,
			DataTable = context.DataTable,
			RowId = context.RowId,
		});
	}

	private RenderFragment _renderOptionsMode(TfSpaceViewColumnOptionsModeContext context)
	{
		return builder =>
		{
			builder.OpenComponent<TucImageViewColumnTypeOptions>(0);
			builder.AddAttribute(1, "Settings", context.GetColumnTypeOptions(new TfImageViewColumnTypeSettings()));
			builder.AddAttribute(2, "ValidationErrors", context.ValidationErrors);
			builder.AddAttribute(4, "SettingsChanged", EventCallback.Factory.Create<TfImageViewColumnTypeSettings>(this, async (options) =>
			{
				context.SetColumnTypeOptions(options);
				await context.SettingsChanged.InvokeAsync(context.ViewColumn.TypeOptionsJson);
			}));
			builder.CloseComponent();
		};
	}

	#endregion

}

public class TfImageViewColumnTypeSettings
{

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string? ChangeConfirmationMessage { get; set; }
}	