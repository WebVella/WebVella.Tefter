namespace WebVella.Tefter.UI.Addons;

public class TfShortIntegerViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>

	public const string ID = "e8a52dfe-fcb8-4fd2-8011-bd9e0a5a81d9";
	public const string NAME = "Short Integer";
	public const string DESCRIPTION = "displays small integer numbers.";
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
				"this column is compatible with integer and short integer database column types",
			SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortInteger }
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
		{
			var settings = context.GetSettings<TfShortIntegerViewColumnTypeSettings>();
			var value = _initValue(args);
			if (value.Count == 0)
			{
				return;
			}
			else if (value.Count == 1)
			{
				if (value[0] is null) return;
				context.ExcelCell.SetValue(XLCellValue.FromObject((short?)value[0]));
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
	public string GetValueAsString(TfSpaceViewColumnBase args)
	{
		if (args is not TfSpaceViewColumnExportCsvMode)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportCsvModeContext is expected");

		if (args is TfSpaceViewColumnExportExcelMode context)
		{
			var settings = context.GetSettings<TfShortIntegerViewColumnTypeSettings>();
			var value = _initValue(args);
			if (value.Count == 0)
			{
				return String.Empty;
			}
			else if (value.Count == 1)
			{
				if (value[0] is null) return String.Empty;
				return ((short)value[0]!).ToString(settings.Format);
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
	private List<short?> _initValue(TfSpaceViewColumnBase args)
	{
		var values = new List<short?>();

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
						values.Add((short?)joinValue);
				}
			}
		}
		else
			values.Add(Convert.ToInt16(columnData));

		return values;
	}

	//Render
	private RenderFragment _renderReadMode(TfSpaceViewColumnReadMode context)
	{
		var values = _initValue(context);
		return builder =>
		{
			builder.OpenComponent<TucShortIntegerViewColumnTypeRead>(0);
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
				builder.OpenComponent<TucShortIntegerViewColumnTypeEdit>(0);
				builder.AddAttribute(1, "Context", context);
				builder.AddAttribute(2, "Value", values[0]);
				builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<short?>(this, async (x) =>
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
			builder.OpenComponent<TucShortIntegerViewColumnTypeOptions>(0);
			builder.AddAttribute(1, "Context", context);
			builder.AddAttribute(2, "SettingsChanged",
				EventCallback.Factory.Create<TfShortIntegerViewColumnTypeSettings>(this,
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

public class TfShortIntegerViewColumnTypeSettings
{
	[JsonPropertyName("ChangeConfirmationMessage")]
	public string? ChangeConfirmationMessage { get; set; }

	[JsonPropertyName("Format")] public string? Format { get; set; }
}