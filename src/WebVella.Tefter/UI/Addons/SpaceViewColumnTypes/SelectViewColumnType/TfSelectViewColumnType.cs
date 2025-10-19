﻿namespace WebVella.Tefter.UI.Addons;

public class TfSelectViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>

	public const string ID = "62c300f7-f8de-49bf-bdcd-f81eac24699a";
	public const string NAME = "Select";
	public const string DESCRIPTION = "displays a selectable options";
	public const string FLUENT_ICON_NAME = "CheckboxChecked";
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
	private List<object?> _initValue(TfSpaceViewColumnBaseContext args)
	{
		var values = new List<object?>();

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
						values.Add(joinValue);
				}
			}
		}
		else
			values.Add(columnData);

		return values;
	}

	private TfSelectOption? _initSelectedOption(object? value, List<TfSelectOption> options)
	{
		TfSelectOption? selectedOption = null;
		var valueJson = JsonSerializer.Serialize(value);
		var optionIndex = options.FindIndex(x => JsonSerializer.Serialize(x.Value) == valueJson);
		if (optionIndex > -1)
		{
			selectedOption = options[optionIndex];
		}
		else if (value is not null)
		{
			options.Insert(0, new TfSelectOption(value, value.ToString()));
			selectedOption = options[0];
		}

		return selectedOption;
	}

	private List<TfSelectOption> _initOptions(TfSpaceViewColumnBaseContext args, TfDataColumn currentColumn)
	{
		var storageKey = this.GetType().FullName + "_" + args.ViewColumn.Id;
		Dictionary<string, object>? viewData = null;
		if (args is TfSpaceViewColumnReadModeContext readContext)
			viewData = readContext.ViewData;
		else if (args is TfSpaceViewColumnEditModeContext editContext)
			viewData = editContext.ViewData;

		if (viewData is not null && viewData.ContainsKey(storageKey))
		{
			if (!viewData.ContainsKey(storageKey) || viewData[storageKey] is not List<TfSelectOption>)
				throw new Exception($"ViewData in the context is not List<TfSelectOption> for key: {storageKey}");
			return (List<TfSelectOption>)viewData[storageKey];
		}

		var settings = args.GetColumnTypeOptions(new TfSelectViewColumnTypeSettings());
		var selectOptions = new List<TfSelectOption>();
		if (settings.Source == TfSelectViewColumnTypeSettingsSourceType.ManuallySet)
		{
			selectOptions = _getOptionsFromString(settings, currentColumn);
		}
		else if (settings.Source == TfSelectViewColumnTypeSettingsSourceType.SpaceData)
		{
			if (settings.DatasetId is not null && settings.DatasetId != Guid.Empty)
			{
				var optionsDt = args.TfService.QueryDataset(
					datasetId: settings.DatasetId.Value,
					userFilters: null,
					userSorts: null,
					search: null,
					page: 1,
					pageSize: TfConstants.SelectOptionsMaxLimit
				);

				for (int i = 0; i < optionsDt.Rows.Count; i++)
				{
					object? value = null;
					string? label = null;
					string? color = null;
					string? backgroundColor = null;
					string? iconName = null;

					if (!String.IsNullOrWhiteSpace(settings.SpaceDataValueColumnName))
					{
						var columnName = settings.SpaceDataValueColumnName.Trim().ToLowerInvariant();
						var column = optionsDt.Columns[columnName];
						if (column is not null)
						{
							value = optionsDt.Rows[i][columnName];
						}
					}

					if (!String.IsNullOrWhiteSpace(settings.SpaceDataLabelColumnName))
					{
						var columnName = settings.SpaceDataLabelColumnName.Trim().ToLowerInvariant();
						var column = optionsDt.Columns[columnName];
						if (column is not null)
						{
							if (optionsDt.Rows[i][columnName] is not null)
								label = optionsDt.Rows[i][columnName]?.ToString();
						}
					}

					if (!String.IsNullOrWhiteSpace(settings.SpaceDataIconColumnName))
					{
						var columnName = settings.SpaceDataIconColumnName.Trim().ToLowerInvariant();
						var column = optionsDt.Columns[columnName];
						if (column is not null)
						{
							if (optionsDt.Rows[i][columnName] is not null)
								iconName = optionsDt.Rows[i][columnName]?.ToString();
						}
					}

					if (!String.IsNullOrWhiteSpace(settings.SpaceDataColorColumnName))
					{
						var columnName = settings.SpaceDataColorColumnName.Trim().ToLowerInvariant();
						var column = optionsDt.Columns[columnName];
						if (column is not null)
						{
							if (optionsDt.Rows[i][columnName] is not null)
							{
								color = TfConverters.GetCssColorFromString(optionsDt.Rows[i][columnName]!.ToString()!);
							}
						}
					}

					if (!String.IsNullOrWhiteSpace(settings.SpaceDataBackgroundColorColumnName))
					{
						var columnName = settings.SpaceDataBackgroundColorColumnName.Trim().ToLowerInvariant();
						var column = optionsDt.Columns[columnName];
						if (column is not null)
						{
							if (optionsDt.Rows[i][columnName] is not null)
							{
								backgroundColor =
									TfConverters.GetCssColorFromString(optionsDt.Rows[i][columnName]!.ToString()!);
							}
						}
					}

					selectOptions.Add(new TfSelectOption(
						value: value,
						label: label,
						iconName: iconName,
						color: color,
						backgroundColor: backgroundColor,
						hideLabel: String.IsNullOrWhiteSpace(label)
					));
				}
			}
		}

		if (viewData is not null)
		{
			viewData[storageKey] = selectOptions;
		}

		return selectOptions;
	}

	private List<TfSelectOption> _getOptionsFromString(TfSelectViewColumnTypeSettings settings,
		TfDataColumn column)
	{
		var result = new List<TfSelectOption>();
		if (String.IsNullOrWhiteSpace(settings.OptionsString)) return result;

		var rows = settings.OptionsString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
		foreach (var row in rows)
		{
			object? value = null;
			string? label = null;
			string? iconName = null;
			string? color = null;
			string? backgroundColor = null;
			bool hideLabel = false;
			var items = row.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
			if (items.Count == 0) continue;
			var valueObj = column.ConvertStringToColumnObject(items[0]);

			if (items.Count >= 1)
			{
				value = valueObj;
			}

			if (items.Count >= 2 && !String.IsNullOrWhiteSpace(items[1]))
			{
				label = items[1];
			}

			if (items.Count >= 3 && !String.IsNullOrWhiteSpace(items[2]))
			{
				iconName = items[2];
			}

			if (items.Count >= 4 && !String.IsNullOrWhiteSpace(items[3]))
			{
				color = TfConverters.GetCssColorFromString(items[3]);
			}

			if (items.Count >= 5 && !String.IsNullOrWhiteSpace(items[4]))
			{
				backgroundColor = TfConverters.GetCssColorFromString(items[4]);
			}

			if (items.Count >= 6 && !String.IsNullOrWhiteSpace(items[5]))
			{
				if (bool.TryParse(items[5], out bool outBool))
				{
					hideLabel = outBool;
				}
			}

			result.Add(new TfSelectOption(
				value: value,
				label: label,
				iconName: iconName,
				color: color,
				backgroundColor: backgroundColor,
				hideLabel: hideLabel
			));
		}

		return result;
	}

	//Render
	private RenderFragment _renderReadMode(TfSpaceViewColumnReadModeContext context)
	{
		var values = _initValue(context);
		var (column, _) = context.GetColumnAndDataByAlias(VALUE_ALIAS);
		var options = _initOptions(context,column);
		var selectedOptions = new List<TfSelectOption?>();
		values.ForEach(x=> selectedOptions.Add(_initSelectedOption(x,options)));
		return builder =>
		{
			builder.OpenComponent<TucSelectViewColumnTypeRead>(0);
			builder.AddAttribute(1, "Value", selectedOptions);
			builder.AddAttribute(2, "Settings", context.GetColumnTypeOptions(new TfSelectViewColumnTypeSettings()));
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
			var options = _initOptions(context,column);
			return builder =>
			{
				builder.OpenComponent<TucSelectViewColumnTypeEdit>(0);
				builder.AddAttribute(1, "Value", _initSelectedOption(values[0], options));
				builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<TfSelectOption?>(this, async (x) =>
				{
					var change = new TfSpaceViewColumnDataChange
					{
						RowId = context.RowId, DataChange = new() { { column.Name, x?.Value } }
					};
					await context.DataChanged.InvokeAsync(change);
				}));
				builder.AddAttribute(3, "Settings", context.GetColumnTypeOptions(new TfSelectViewColumnTypeSettings()));
				builder.AddAttribute(4, "Options", options);
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
		List<TfDataset> datasets = context.TfService.GetDatasets();
		var settings = context.GetColumnTypeOptions(new TfSelectViewColumnTypeSettings());
		TfDataset? dataset = null;
		if(settings.DatasetId is not null)
			dataset = context.TfService.GetDataset(settings.DatasetId.Value);
		if (dataset is null && datasets.Count > 0)
			dataset = datasets[0];
		
		return builder =>
		{
			builder.OpenComponent<TucSelectViewColumnTypeOptions>(0);
			builder.AddAttribute(1, "Settings", settings);
			builder.AddAttribute(2, "ValidationErrors", context.ValidationErrors);
			builder.AddAttribute(4, "SettingsChanged", EventCallback.Factory.Create<TfSelectViewColumnTypeSettings>(
				this, async (options) =>
				{
					Console.WriteLine("SettingsChanged");
					context.SetColumnTypeOptions(options);
					await context.SettingsChanged.InvokeAsync(context.ViewColumn.TypeOptionsJson);
				}));
			builder.AddAttribute(5, "SelectedDataset", dataset);
			builder.AddAttribute(6, "DatasetOptions", datasets);
			builder.CloseComponent();
		};
	}

	#endregion
}

public class TfSelectViewColumnTypeSettings
{
	[JsonPropertyName("ChangeConfirmationMessage")]
	public string? ChangeConfirmationMessage { get; set; }

	[JsonPropertyName("NotSelectedString")] public string? NotSelectedString { get; set; }	
	
	[JsonPropertyName("Source")]
	public TfSelectViewColumnTypeSettingsSourceType Source { get; set; } =
		TfSelectViewColumnTypeSettingsSourceType.ManuallySet;

	[JsonPropertyName("OptionsString")] public string? OptionsString { get; set; }

	[JsonPropertyName("SpaceDataId")] public Guid? DatasetId { get; set; }

	[JsonPropertyName("SpaceDataValueColumnName")]
	public string? SpaceDataValueColumnName { get; set; }

	[JsonPropertyName("SpaceDataLabelColumnName")]
	public string? SpaceDataLabelColumnName { get; set; }

	[JsonPropertyName("SpaceDataColorColumnName")]
	public string? SpaceDataColorColumnName { get; set; }

	[JsonPropertyName("SpaceDataBackgroundColorColumnName")]
	public string? SpaceDataBackgroundColorColumnName { get; set; }

	[JsonPropertyName("SpaceDataIconColumnName")]
	public string? SpaceDataIconColumnName { get; set; }
}

public enum TfSelectViewColumnTypeSettingsSourceType
{
	[Description("manually set")] ManuallySet = 0,
	[Description("space dataset")] SpaceData = 1,
}