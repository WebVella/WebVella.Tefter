namespace WebVella.Tefter.UI.Addons;

public class TfTextViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>

	public const string ID = "f061a3ce-7813-4fd6-98cb-a10cccea4797";
	public const string NAME = "Text";
	public const string DESCRIPTION = "displays text";
	public const string FLUENT_ICON_NAME = "TextCaseTitle";
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

	#region << PUBLIC >>

	public void ProcessExcelCell(TfSpaceViewColumnBaseContext args)
	{
		if (args is not TfSpaceViewColumnExportExcelModeContext)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportExcelModeContext is expected");
		if (args is TfSpaceViewColumnExportExcelModeContext context)
			context.ExcelCell.SetValue(XLCellValue.FromObject(String.Join(", ", _initValues(args))));
	}

	//Returns Value/s as string usually for CSV export
	public string? GetValueAsString(TfSpaceViewColumnBaseContext args)
	{
		if (args is not TfSpaceViewColumnExportCsvModeContext)
			throw new Exception("Wrong context type. TfSpaceViewColumnExportCsvModeContext is expected");

		return String.Join(", ", _initValues(args));
	}

	public RenderFragment? Render(TfSpaceViewColumnBaseContext args)
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

	#endregion

	#region << Private >>

	//Value
	private List<string?> _initValues(TfSpaceViewColumnBaseContext args)
	{
		var values = new List<string?>();

		var (column, columnData) = args.GetColumnAndDataByAlias(VALUE_ALIAS);
		if (columnData is null)
		{
			values.Add(String.Empty);
		}
		else if (column.OriginType == TfDataColumnOriginType.JoinedProviderColumn)
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
	private RenderFragment? _renderReadMode(TfSpaceViewColumnReadModeContext context)
	{
		var values = _initValues(context);
		return builder =>
		{
			builder.OpenComponent<TucTextViewColumnTypeRead>(0);
			builder.AddAttribute(1, "Value", values);
			builder.CloseComponent();
		};
	}

	private RenderFragment? _renderEditMode(TfSpaceViewColumnEditModeContext context)
	{
		var (column, columnData) = context.GetColumnAndDataByAlias(VALUE_ALIAS);

		//Editable columns
		if (column.OriginType == TfDataColumnOriginType.CurrentProvider
		    || column.OriginType == TfDataColumnOriginType.SharedColumn)
		{
			var values = _initValues(context);

			return builder =>
			{
				builder.OpenComponent<TucTextViewColumnTypeEdit>(0);
				builder.AddAttribute(1, "Value", values);
				builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create(this, async (x) =>
				{
					var (column, columnData) = context.GetColumnAndDataByAlias(VALUE_ALIAS);
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
			SpaceViewColumn = context.SpaceViewColumn,
			DataTable = context.DataTable,
			RowId = context.RowId,
		});
	}

	private RenderFragment? _renderOptionsMode(TfSpaceViewColumnOptionsModeContext context)
	{
		return builder =>
		{
			builder.OpenElement(0, "span");
			builder.AddAttribute(1, "style", "color:var(--error)");
			builder.AddContent(2, "error");
			builder.CloseElement();
		};
	}

	#endregion
}