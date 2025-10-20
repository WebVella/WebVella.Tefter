namespace WebVella.Tefter.UI.Addons;

public class TfNumberViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>

	public const string ID = "5d246be4-d202-434c-961e-204e44ee0450";
	public const string NAME = "Number";
	public const string DESCRIPTION = "displays decimal numbers";
	public const string FLUENT_ICON_NAME = "NumberSymbol";
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
				"this column is compatible with Number and short Number database column types",
			SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }
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
		{
			var settings = context.GetSettings<TfNumberViewColumnTypeSettings>();
			var value = _initValue(args);
			if (value.Count == 0)
			{
				return;
			}
			else if (value.Count == 1)
			{
				if (value[0] is null) return;
				context.ExcelCell.SetValue(XLCellValue.FromObject((decimal?)value[0]));
			}
			else
			{
				var valuesList = new List<string>();
				foreach (var item in value)
				{
					if (item is null)
					{
						valuesList.Add(TfConstants.ExcelNullWord);
						continue;
					}

					valuesList.Add(item.Value.ToString(settings.Format));
				}

				context.ExcelCell.SetValue(XLCellValue.FromObject(String.Join(", ", valuesList)));
			}
		}
	}

	//Returns Value/s as string usually for CSV export
	public string GetValueAsString(TfSpaceViewColumnBaseContext args)
	{
		if (args is not TfSpaceViewColumnExportCsvModeContext)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportCsvModeContext is expected");

		if (args is TfSpaceViewColumnExportExcelModeContext context)
		{
			var settings = context.GetSettings<TfNumberViewColumnTypeSettings>();
			var value = _initValue(args);
			if (value.Count == 0)
			{
				return String.Empty;
			}
			else if (value.Count == 1)
			{
				if (value[0] is null) return String.Empty;
				return ((decimal)value[0]!).ToString(settings.Format);
			}
			else
			{
				var valuesList = new List<string>();
				foreach (var item in value)
				{
					if (item is null)
					{
						valuesList.Add(TfConstants.ExcelNullWord);
						continue;
					}

					valuesList.Add(item.Value.ToString(settings.Format));
				}

				return String.Join(", ", valuesList);
			}
		}

		return String.Empty;
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
	private List<decimal?> _initValue(TfSpaceViewColumnBaseContext args)
	{
		var values = new List<decimal?>();

		var (column, columnData) = args.GetColumnAndDataByAlias(VALUE_ALIAS);
		if (column is null) return values;
		if (columnData is null)
		{
			values.Add(null);
		}
		else if (column.Origin == TfDataColumnOriginType.JoinedProviderColumn)
		{
			if (columnData.GetType().ImplementsInterface(typeof(IList)))
			{
				if (columnData is IList joinValues)
				{
					foreach (var joinValue in joinValues)
						values.Add((decimal?)joinValue);
				}
			}
		}
		else
			values.Add(Convert.ToDecimal(columnData));

		return values;
	}

	//Render
	private RenderFragment _renderReadMode(TfSpaceViewColumnReadModeContext context)
	{
		var values = _initValue(context);
		return builder =>
		{
			builder.OpenComponent<TucNumberViewColumnTypeRead>(0);
			builder.AddAttribute(1, "Context", context);
			builder.AddAttribute(2, "Value", values);
			builder.CloseComponent();
		};
	}

	private RenderFragment _renderEditMode(TfSpaceViewColumnEditModeContext context)
	{
		var (column, _) = context.GetColumnAndDataByAlias(VALUE_ALIAS);
		if (column is null)
			return builder =>
			{
				builder.OpenElement(0, "span");
				builder.CloseElement();
			};
		//Editable columns
		if (column.Origin == TfDataColumnOriginType.CurrentProvider
		    || column.Origin == TfDataColumnOriginType.SharedColumn)
		{
			var values = _initValue(context);

			return builder =>
			{
				builder.OpenComponent<TucNumberViewColumnTypeEdit>(0);
				builder.AddAttribute(1, "Context", context);
				builder.AddAttribute(2, "Value", values[0]);
				builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<decimal?>(this, async (x) =>
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
			builder.OpenComponent<TucNumberViewColumnTypeOptions>(0);
			builder.AddAttribute(1, "Context", context);
			builder.AddAttribute(2, "SettingsChanged", EventCallback.Factory.Create<TfNumberViewColumnTypeSettings>(
				this,
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

public class TfNumberViewColumnTypeSettings
{
	[JsonPropertyName("ChangeConfirmationMessage")]
	public string? ChangeConfirmationMessage { get; set; }

	[JsonPropertyName("Format")] public string? Format { get; set; }
}