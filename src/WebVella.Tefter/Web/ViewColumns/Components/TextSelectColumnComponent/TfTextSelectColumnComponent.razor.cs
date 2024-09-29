namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Text Select")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.TextSelectColumnComponent.TfTextSelectColumnComponent", "WebVella.Tefter")]
public partial class TfTextSelectColumnComponent : TfBaseViewColumn<TfTextSelectColumnComponentOptions>
{
	#region << Injects >>
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	#endregion

	#region << Constructor >>

	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfTextSelectColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTextSelectColumnComponent(TfComponentContext context)
	{
		Context = context;
	}
	#endregion

	#region << Properties >>
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private object _value = null;
	private string _valueInputId = "input-" + Guid.NewGuid();
	private List<TucSelectOption> _selectOptionsList = new();
	private TucSelectOption _selectedOption = null;
	private bool _open = false;
	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private Guid? _renderedHash = null;
	private string _storageKey = "";
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
		if (Context.Hash != _renderedHash)
		{
			await _initValues();
			_renderedHash = Context.Hash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataStringByAlias(_valueAlias);
	}

	public override async Task OnSpaceViewStateInited(
		IDataManager dataManager,
		ITfSpaceManager spaceManager,
		TucUser currentUser,
		TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		await base.OnSpaceViewStateInited(
			dataManager: dataManager,
			spaceManager: spaceManager,
			currentUser: currentUser,
			routeState: routeState,
			newAppState: newAppState,
			oldAppState: oldAppState,
			newAuxDataState: newAuxDataState,
			oldAuxDataState: oldAuxDataState
		);
		_initStorageKeys();
		var options = new List<TucSelectOption>();
		var componentOptions = GetOptions();
		if (componentOptions.Source == TfTextSelectColumnComponentOptionsSourceType.ManuallySet)
		{
			if (!String.IsNullOrWhiteSpace(componentOptions.OptionsString))
			{
				var rows = componentOptions.OptionsString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
				foreach (var row in rows)
				{
					var items = row.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
					if (items.Count == 0) continue;
					var valueObj = ConvertStringToColumnObjectByAlias(_valueAlias, items[0]);

					if (items.Count == 1)
					{
						options.Add(new TucSelectOption(valueObj, items[0]));
					}
					else if (items.Count > 1)
					{
						options.Add(new TucSelectOption(valueObj, items[1]));
					}

				}
			}

		}
		else if (componentOptions.Source == TfTextSelectColumnComponentOptionsSourceType.SpaceData)
		{
			if (componentOptions.SpaceDataId != Guid.Empty)
			{
				var optionsDTResult = dataManager.QuerySpaceData(
					spaceDataId: componentOptions.SpaceDataId,
					additionalFilters: null,
					sortOverrides: null,
					search: null,
					page: 1,
					pageSize: TfConstants.SelectOptionsMaxLimit
				);
				var optionsDT = optionsDTResult.Value;
				for (int i = 0; i < optionsDT.Rows.Count; i++)
				{
					object value = null;
					string label = null;
					string color = null;
					string backgroundColor = null;
					string iconName = null;

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

					options.Add(new TucSelectOption(value, label));
				}
			}
		}
		newAuxDataState.Data[_storageKey] = options;

	}
	#endregion

	#region << Private logic >>
	/// <summary>
	/// process the value change event from the components view
	/// by design if any kind of error occurs the old value should be set back
	/// so the user is notified that the change is aborted
	/// </summary>
	/// <returns></returns>
	private async Task _valueChanged()
	{
		if (componentOptions.ChangeRequiresConfirmation)
		{
			var message = componentOptions.ChangeConfirmationMessage;
			if (String.IsNullOrWhiteSpace(message))
				message = LOC("Please confirm the data change!");

			if (!await JSRuntime.InvokeAsync<bool>("confirm", message))
			{
				await InvokeAsync(StateHasChanged);
				await Task.Delay(10);
				await _initValues();
				await InvokeAsync(StateHasChanged);
				return;
			};
		}

		try
		{
			await OnRowColumnChangedByAlias(_valueAlias, _value);
			ToastService.ShowSuccess(LOC("change applied"));
			await JSRuntime.InvokeAsync<string>("Tefter.blurElement", _valueInputId);
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
			await InvokeAsync(StateHasChanged);
			await Task.Delay(10);
			await _initValues();
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _optionChanged(TucSelectOption option)
	{
		if (option is null && _value is null
		|| (option is not null && option.Value == _value)) return;
		if (option is null) _value = null;
		else _value = option.Value;
		await _valueChanged();
	}
	private async Task _initValues()
	{
		_value = GetColumnDataByAlias(_valueAlias);
		if (!TfAuxDataState.Value.Data.ContainsKey(_storageKey)) return;
		_selectOptionsList = ((List<TucSelectOption>)TfAuxDataState.Value.Data[_storageKey]).ToList();
		var column = GetColumnInfoByAlias(_valueAlias);
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

		if (Context.Mode == TfComponentMode.Options)
		{
			var spaceDataIndex = TfAppState.Value.SpaceDataList?.FindIndex(x => x.Id == componentOptions.SpaceDataId);

			if (spaceDataIndex == -1 && TfAppState.Value.SpaceDataList is not null && TfAppState.Value.SpaceDataList.Count > 0)
			{
				await OnOptionsChanged(nameof(componentOptions.SpaceDataId), TfAppState.Value.SpaceDataList[0].Id);
			}
		}
		//_assignActions();
	}

	private void _initStorageKeys()
	{
		_storageKey = this.GetType().Name + "_" + Context.SpaceViewColumnId;
	}

	private void _assignActions()
	{
		if (_selectOptionsList is null) return;
		foreach (var option in _selectOptionsList)
		{
			option.OnClick = async () => await _optionChanged(option);
		}
	}
	#endregion
}

public class TfTextSelectColumnComponentOptions
{
	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }

	[JsonPropertyName("Source")]
	public TfTextSelectColumnComponentOptionsSourceType Source { get; set; } = TfTextSelectColumnComponentOptionsSourceType.ManuallySet;

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

public enum TfTextSelectColumnComponentOptionsSourceType
{
	[Description("manually set")]
	ManuallySet = 0,
	[Description("space data")]
	SpaceData = 1,
}