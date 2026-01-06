namespace WebVella.Tefter.UI.Addons;

public class TfUrlViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>

	public const string ID = "75fa0e5e-24d8-4971-854c-d0ed41ad9019";
	public const string NAME = "URL Display";
	public const string DESCRIPTION = "displays an url";
	public const string FLUENT_ICON_NAME = "Link";
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

	public void ProcessExcelCell(TfSpaceViewColumnBase args)
	{
		if (args is not TfSpaceViewColumnExportExcelMode)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportExcelModeContext is expected");
		if (args is TfSpaceViewColumnExportExcelMode context)
			context.ExcelCell.SetValue(XLCellValue.FromObject(String.Join(", ", _initValue(args))));
	}

	//Returns Value/s as string usually for CSV export
	public string GetValueAsString(TfSpaceViewColumnBase args)
	{
		if (args is not TfSpaceViewColumnExportCsvMode)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportCsvModeContext is expected");

		return String.Join(", ", _initValue(args));
	}

	public RenderFragment Render(TfSpaceViewColumnBase args)
	{
		if (args is null)
			throw new Exception("TfSpaceViewColumnBaseContext is expected");
		if (args is TfSpaceViewColumnReadMode readContext)
			return _renderReadMode(readContext);
		if (args is TfSpaceViewColumnEditMode editContext)
			return _renderEditMode(editContext);
		if (args is TfSpaceViewColumnOptionsMode optionsContext)
			return _renderOptionsMode(optionsContext);

		throw new Exception("Unsupported render mode");
	}

	public List<ValidationError> ValidateTypeOptions(TfSpaceViewColumnOptionsMode args)
	{
		_validationErrors = new();
		return _validationErrors;
	}

	#endregion

	#region << Private >>

	//Value
	private List<string?> _initValue(TfSpaceViewColumnBase args)
	{
		var values = new List<string?>();

		var (column, columnData) = args.GetColumnAndDataByAlias(VALUE_ALIAS);
		if (column is null) return values;
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
	private RenderFragment _renderReadMode(TfSpaceViewColumnReadMode context)
	{
		var values = _initValue(context);
		return builder =>
		{
			builder.OpenComponent<TucUrlViewColumnTypeRead>(0);
			builder.AddAttribute(1, "Context", context);
			builder.AddAttribute(2, "Value", values);
			builder.CloseComponent();
		};
	}

	private RenderFragment _renderEditMode(TfSpaceViewColumnEditMode context)
	{
		var (column, _) = context.GetColumnAndDataByAlias(VALUE_ALIAS);
		if (column is null)
			return builder =>
			{
				builder.OpenElement(0, "span");
				builder.CloseElement();
			};
		//Editable columns
		if (!column.IsReadOnly)
		{
			var values = _initValue(context);

			return builder =>
			{
				builder.OpenComponent<TucUrlViewColumnTypeEdit>(0);
				builder.AddAttribute(1, "Context", context);
				builder.AddAttribute(2, "Value", values[0]);
				builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string?>(this, async (x) =>
				{
					var change = new TfSpaceViewColumnDataChange
					{
						RowId = context.RowId, DataChange = new() { { column.Name, x } }
					};
					await context.DataChanged.InvokeAsync(change);
				}));
				builder.CloseComponent();
			};
		}

		//Non Editable columns
		return _renderReadMode(new TfSpaceViewColumnReadMode(context.ViewData)
		{
			TfService = context.TfService,
			ViewColumn = context.ViewColumn,
			DataTable = context.DataTable,
			RowId = context.RowId,
		});
	}

	private RenderFragment _renderOptionsMode(TfSpaceViewColumnOptionsMode context)
	{
		return builder =>
		{
			builder.OpenComponent<TucUrlViewColumnTypeOptions>(0);
			builder.AddAttribute(1, "Context", context);
			builder.AddAttribute(2, "SettingsChanged", EventCallback.Factory.Create<TfUrlViewColumnTypeSettings>(this,
				async (options) =>
				{
					context.SetColumnTypeOptions(options);
					await context.SettingsChanged.InvokeAsync(context.ViewColumn.TypeOptionsJson);
				}));
			builder.CloseComponent();
		};
	}

	#endregion
}

public class TfUrlViewColumnTypeSettings
{
	[JsonPropertyName("ChangeConfirmationMessage")]
	public string? ChangeConfirmationMessage { get; set; }
}