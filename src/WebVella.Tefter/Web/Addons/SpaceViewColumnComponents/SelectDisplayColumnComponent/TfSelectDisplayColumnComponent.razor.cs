﻿namespace WebVella.Tefter.Web.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponents.SelectDisplayColumnComponent.TfSelectDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfSelectDisplayColumnComponent : TucBaseViewColumn<TfSelectDisplayColumnComponentOptions>
{
	public const string ID = "dd1cf064-9167-4432-b603-128b931ca89d";
	public const string NAME = "Select Display";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";
	public const string VALUE_ALIAS = "Value";

	#region << Injects >>
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	#endregion

	#region << Constructor >>

	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfSelectDisplayColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfSelectDisplayColumnComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid AddonId { get; init; } = new Guid(ID);
	public override string AddonName { get; init; } = NAME;
	public override string AddonDescription { get; init; } = DESCRIPTION;
	public override string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
		new Guid(TfTextViewColumnType.ID),
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnTypeAddon that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private object _value = null;
	private List<TucSelectOption> _selectOptionsList = new();
	private TucSelectOption _selectedOption = null;
	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private string _renderedHash = null;
	private string _storageKey = "";
	private TucSpaceData _selectedSpaceData = null;
	#endregion

	#region << Lifecycle >>
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initStorageKeys();
	}

	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		var contextHash = RegionContext.GetHash();
		if (contextHash != _renderedHash)
		{
			await _initValues();
			_renderedHash = contextHash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell)
	{
		excelCell.SetValue(XLCellValue.FromObject(GetDataStringByAlias(VALUE_ALIAS)));
	}

	public override async Task OnAppStateInit(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		await base.OnAppStateInit(
			serviceProvider: serviceProvider,
			currentUser: currentUser,
			newAppState: newAppState,
			oldAppState: oldAppState,
			newAuxDataState: newAuxDataState,
			oldAuxDataState: oldAuxDataState
		);
		_initStorageKeys();
		var options = new List<TucSelectOption>();
		var componentOptions = GetOptions();
		if (componentOptions.Source == TfSelectDisplayColumnComponentOptionsSourceType.ManuallySet)
		{
			options = _getOptionsFromString(componentOptions.OptionsString);
		}
		else if (componentOptions.Source == TfSelectDisplayColumnComponentOptionsSourceType.SpaceData)
		{
			if (componentOptions.SpaceDataId != Guid.Empty)
			{
				var tfService = serviceProvider.GetService<ITfService>();

				var optionsDT = tfService.QuerySpaceData(
					spaceDataId: componentOptions.SpaceDataId,
					userFilters: null,
					userSorts: null,
					search: null,
					page: 1,
					pageSize: TfConstants.SelectOptionsMaxLimit
				);

				for (int i = 0; i < optionsDT.Rows.Count; i++)
				{
					object value = null;
					string label = null;
					string color = null;
					string backgroundColor = null;
					string iconName = null;
					bool hideLabel = componentOptions.SpaceDataHideLabel;

					if (!String.IsNullOrWhiteSpace(componentOptions.SpaceDataValueColumnName))
					{
						var columnName = componentOptions.SpaceDataValueColumnName.Trim().ToLowerInvariant();
						var column = optionsDT.Columns[columnName];
						if (column is not null)
						{
							value = optionsDT.Rows[i][columnName];
						}
					}
					if (!String.IsNullOrWhiteSpace(componentOptions.SpaceDataLabelColumnName))
					{
						var columnName = componentOptions.SpaceDataLabelColumnName.Trim().ToLowerInvariant();
						var column = optionsDT.Columns[columnName];
						if (column is not null)
						{
							if (optionsDT.Rows[i][columnName] is not null)
								label = optionsDT.Rows[i][columnName].ToString();
						}
					}
					if (!String.IsNullOrWhiteSpace(componentOptions.SpaceDataIconColumnName))
					{
						var columnName = componentOptions.SpaceDataIconColumnName.Trim().ToLowerInvariant();
						var column = optionsDT.Columns[columnName];
						if (column is not null)
						{
							if (optionsDT.Rows[i][columnName] is not null)
								iconName = optionsDT.Rows[i][columnName].ToString();
						}
					}
					if (!String.IsNullOrWhiteSpace(componentOptions.SpaceDataColorColumnName))
					{
						var columnName = componentOptions.SpaceDataColorColumnName.Trim().ToLowerInvariant();
						var column = optionsDT.Columns[columnName];
						if (column is not null)
						{
							if (optionsDT.Rows[i][columnName] is not null)
							{
								color = TfConverters.GetCssColorFromString(optionsDT.Rows[i][columnName].ToString());
							}
						}
					}
					if (!String.IsNullOrWhiteSpace(componentOptions.SpaceDataBackgroundColorColumnName))
					{
						var columnName = componentOptions.SpaceDataBackgroundColorColumnName.Trim().ToLowerInvariant();
						var column = optionsDT.Columns[columnName];
						if (column is not null)
						{
							if (optionsDT.Rows[i][columnName] is not null)
							{
								backgroundColor = TfConverters.GetCssColorFromString(optionsDT.Rows[i][columnName].ToString());
							}
						}
					}

					options.Add(new TucSelectOption(
						value: value,
						label: label,
						iconName: iconName,
						color: color,
						backgroundColor: backgroundColor,
						hideLabel: hideLabel
					));
				}
			}
		}
		newAuxDataState.Data[_storageKey] = options;

	}
	#endregion

	#region << Private logic >>
	private async Task _initValues()
	{
		if (RegionContext.Mode == TfComponentPresentationMode.Options)
		{
			_selectedSpaceData = TfAppState.Value.SpaceDataList?.FirstOrDefault(x => x.Id == componentOptions.SpaceDataId);

			if (_selectedSpaceData is null && TfAppState.Value.SpaceDataList is not null && TfAppState.Value.SpaceDataList.Count > 0)
			{
				_selectedSpaceData = TfAppState.Value.SpaceDataList[0];
				await OnOptionsChanged(nameof(componentOptions.SpaceDataId), _selectedSpaceData.Id);
			}
			return;
		}
		TfDataColumn column = GetColumnByAlias(VALUE_ALIAS);
		if (column is null)
			throw new Exception("Column not found");
		if (column.IsJoinColumn)
			throw new Exception("Joined data cannot be edited");

		_value = GetColumnDataByAlias(VALUE_ALIAS);
		if (!TfAuxDataState.Value.Data.ContainsKey(_storageKey)) return;
		_selectOptionsList = ((List<TucSelectOption>)TfAuxDataState.Value.Data[_storageKey]).ToList();
		if (column is not null && column.IsNullable)
		{
			_selectOptionsList.Insert(0, new TucSelectOption(null, LOC("no value")));
		}

		_selectedOption = null;
		var valueJson = JsonSerializer.Serialize(_value);
		var optionIndex = _selectOptionsList.FindIndex(x => JsonSerializer.Serialize(x.Value) == valueJson);
		if (optionIndex > -1)
		{
			_selectedOption = _selectOptionsList[optionIndex];
		}
		else if (_value is not null)
		{
			_selectOptionsList.Insert(0, new TucSelectOption(_value, _value.ToString()));
			_selectedOption = _selectOptionsList[0];
		}

	}
	private void _initStorageKeys()
	{
		_storageKey = this.GetType().Name + "_" + RegionContext.SpaceViewColumnId;
	}
	private List<TucSelectOption> _getOptionsFromString(string optionsString)
	{
		var result = new List<TucSelectOption>();
		if (String.IsNullOrWhiteSpace(componentOptions.OptionsString)) return result;

		var rows = componentOptions.OptionsString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
		foreach (var row in rows)
		{
			object value = null;
			string label = null;
			string iconName = null;
			string color = null;
			string backgroundColor = null;
			bool hideLabel = false;
			var items = row.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
			if (items.Count == 0) continue;
			var valueObj = ConvertStringToColumnObjectByAlias(VALUE_ALIAS, items[0]);

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

			result.Add(new TucSelectOption(
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

	private async Task _spaceDataChanged(TucSpaceData spaceData)
	{
		_selectedSpaceData = spaceData;
		await OnOptionsChanged(nameof(componentOptions.SpaceDataId), _selectedSpaceData?.Id);
	}


	private string _getStyle()
	{
		var sb = new StringBuilder();
		sb.Append("width:100%;");
		if (!String.IsNullOrWhiteSpace(_selectedOption?.Color))
			sb.Append($"color:{_selectedOption.Color};");
		if (!String.IsNullOrWhiteSpace(_selectedOption?.BackgroundColor))
			sb.Append($"background-color:{_selectedOption.BackgroundColor};");

		return sb.ToString();
	}

	#endregion
}

public class TfSelectDisplayColumnComponentOptions
{

	[JsonPropertyName("Source")]
	public TfSelectDisplayColumnComponentOptionsSourceType Source { get; set; } = TfSelectDisplayColumnComponentOptionsSourceType.ManuallySet;

	[JsonPropertyName("OptionsString")]
	public string OptionsString { get; set; }

	[JsonPropertyName("SpaceDataId")]
	public Guid SpaceDataId { get; set; }

	[JsonPropertyName("SpaceDataHideLabel")]
	public bool SpaceDataHideLabel { get; set; } = false;

	[JsonPropertyName("SpaceDataValueColumnName")]
	public string SpaceDataValueColumnName { get; set; }

	[JsonPropertyName("SpaceDataLabelColumnName")]
	public string SpaceDataLabelColumnName { get; set; }

	[JsonPropertyName("SpaceDataColorColumnName")]
	public string SpaceDataColorColumnName { get; set; }

	[JsonPropertyName("SpaceDataBackgroundColorColumnName")]
	public string SpaceDataBackgroundColorColumnName { get; set; }

	[JsonPropertyName("SpaceDataIconColumnName")]
	public string SpaceDataIconColumnName { get; set; }
}

public enum TfSelectDisplayColumnComponentOptionsSourceType
{
	[Description("manually set")]
	ManuallySet = 0,
	[Description("space dataset")]
	SpaceData = 1,
}