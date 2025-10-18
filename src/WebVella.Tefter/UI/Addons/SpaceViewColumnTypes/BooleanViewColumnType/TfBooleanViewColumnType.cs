namespace WebVella.Tefter.UI.Addons;

public class TfBooleanViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>

	public const string ID = "c28e933b-6800-4819-b22f-e091e3e3c961";
	public const string NAME = "Boolean";
	public const string DESCRIPTION = "displays a boolean";
	public const string FLUENT_ICON_NAME = "CircleMultipleSubtractCheckmark";
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
				"this column is compatible with the Boolean database column type",
			SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType>
			{
				TfDatabaseColumnType.Boolean
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
		{
			var settings = context.GetColumnTypeOptions(new TfBooleanViewColumnTypeSettings());
			var value = _initValue(args);
			if (value.Count == 0)
			{
				return;
			}
			else if (value.Count == 1)
			{
				if (value[0] is null) return;
				if (!String.IsNullOrWhiteSpace(settings.TrueLabel))
					context.ExcelCell.SetValue(XLCellValue.FromObject(settings.TrueLabel));
				else if (!String.IsNullOrWhiteSpace(settings.FalseLabel))
					context.ExcelCell.SetValue(XLCellValue.FromObject(settings.FalseLabel));
				else
					context.ExcelCell.SetValue(XLCellValue.FromObject((bool?)value[0]));
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
					if (!String.IsNullOrWhiteSpace(settings.TrueLabel))
						valuesList.Add(settings.TrueLabel);
					else if (!String.IsNullOrWhiteSpace(settings.FalseLabel))
						valuesList.Add(settings.FalseLabel);
					else
						valuesList.Add(item.Value.ToString());
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
			var settings = context.GetColumnTypeOptions(new TfBooleanViewColumnTypeSettings());
			var value = _initValue(args);
			if (value.Count == 0)
			{
				return String.Empty;
			}
			else if (value.Count == 1)
			{
				if (value[0] is null) return String.Empty;
				if (!String.IsNullOrWhiteSpace(settings.TrueLabel))
					return settings.TrueLabel;
				else if (!String.IsNullOrWhiteSpace(settings.FalseLabel))
					return settings.FalseLabel;
				else
					return ((bool)value[0]!).ToString().ToLowerInvariant();
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
					if (!String.IsNullOrWhiteSpace(settings.TrueLabel))
						valuesList.Add(settings.TrueLabel);
					else if (!String.IsNullOrWhiteSpace(settings.FalseLabel))
						valuesList.Add(settings.FalseLabel);
					else
						valuesList.Add(item.Value.ToString());
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
	private List<bool?> _initValue(TfSpaceViewColumnBaseContext args)
	{
		var values = new List<bool?>();

		var (column, columnData) = args.GetColumnAndDataByAlias(VALUE_ALIAS);
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
						values.Add((bool?)joinValue);
				}
			}
		}
		else
			values.Add((bool?)columnData);

		return values;
	}

	//Render
	private RenderFragment _renderReadMode(TfSpaceViewColumnReadModeContext context)
	{
		var values = _initValue(context);
		return builder =>
		{
			builder.OpenComponent<TucBooleanViewColumnTypeRead>(0);
			builder.AddAttribute(1, "Value", values);
			builder.AddAttribute(2, "Settings", context.GetColumnTypeOptions(new TfBooleanViewColumnTypeSettings()));
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
				builder.OpenComponent<TucBooleanViewColumnTypeEdit>(0);
				builder.AddAttribute(1, "Value", values[0]);
				builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<bool?>(this, async (x) =>
				{
					var change = new TfSpaceViewColumnDataChange
					{
						RowId = context.RowId, DataChange = new() { { column.Name, x } }
					};
					await context.DataChanged.InvokeAsync(change);
				}));
				builder.AddAttribute(3, "Settings", context.GetColumnTypeOptions(new TfBooleanViewColumnTypeSettings()));
				builder.AddAttribute(4, "IsNullableColumn", column.IsNullable);
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
			builder.OpenComponent<TucBooleanViewColumnTypeOptions>(0);
			builder.AddAttribute(1, "Settings", context.GetColumnTypeOptions(new TfBooleanViewColumnTypeSettings()));
			builder.AddAttribute(2, "ValidationErrors", context.ValidationErrors);
			builder.AddAttribute(4, "SettingsChanged", EventCallback.Factory.Create<TfBooleanViewColumnTypeSettings>(this, async (options) =>
			{
				context.SetColumnTypeOptions(options);
				await context.SettingsChanged.InvokeAsync(context.ViewColumn.TypeOptionsJson);
			}));
			builder.CloseComponent();
		};
	}

	#endregion	
}

public class TfBooleanViewColumnTypeSettings
{
	[JsonPropertyName("ShowLabel")]
	public bool ShowLabel { get; set; } = true;
	
	[JsonPropertyName("TrueLabel")]
	public string? TrueLabel { get; set; }

	[JsonPropertyName("TrueValueShowAsIcon")]
	public bool TrueValueShowAsIcon { get; set; }

	[JsonPropertyName("FalseLabel")]
	public string? FalseLabel { get; set; }

	[JsonPropertyName("FalseValueShowAsIcon")]
	public bool FalseValueShowAsIcon { get; set; }

	[JsonPropertyName("NullLabel")]
	public string? NullLabel { get; set; }

	[JsonPropertyName("NullValueShowAsIcon")]
	public bool NullValueShowAsIcon { get; set; }
	
	[JsonPropertyName("ChangeConfirmationMessage")]
	public string? ChangeConfirmationMessage { get; set; }	

}	