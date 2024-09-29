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
	private List<Tuple<object, string>> _selectOptionsList = new();
	private Tuple<object, string> _selectedOption = null;
	private bool _open = false;
	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private Guid? _renderedHash = null;
	private string _storageKey = "";
	private TucSpaceData _selectedSpaceData = null;
	#endregion

	#region << Lifecycle >>
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_initStorageKeys();
		if (TfAppState.Value.SpaceDataList is not null && TfAppState.Value.SpaceDataList.Count > 0)
			_selectedSpaceData = TfAppState.Value.SpaceDataList[0];
	}

	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Hash != _renderedHash)
		{
			_initValues();
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

	public override async Task OnSpaceViewStateInited(TucUser currentUser,
		TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		await base.OnSpaceViewStateInited(
			currentUser: currentUser,
			routeState: routeState,
			newAppState: newAppState,
			oldAppState: oldAppState,
			newAuxDataState: newAuxDataState,
			oldAuxDataState: oldAuxDataState
		);
		_initStorageKeys();
		var options = new List<Tuple<object, string>>();
		var componentOptions = GetOptions();
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
					options.Add(new Tuple<object, string>(valueObj, items[0]));
				}
				else if (items.Count > 1)
				{
					options.Add(new Tuple<object, string>(valueObj, items[1]));
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
				_initValues();
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
			_initValues();
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _optionChanged(Tuple<object, string> option)
	{
		if (option is null && _value is null
		|| (option is not null && option.Item1 == _value)) return;
		if (option is null) _value = null;
		else _value = option.Item1;
		await _valueChanged();
	}
	private void _initValues()
	{
		_value = GetColumnDataByAlias(_valueAlias);
		if (!TfAuxDataState.Value.Data.ContainsKey(_storageKey)) return;
		_selectOptionsList = ((List<Tuple<object, string>>)TfAuxDataState.Value.Data[_storageKey]).ToList();
		var column = GetColumnInfoByAlias(_valueAlias);
		if (column is not null && column.IsNullable)
		{
			_selectOptionsList.Insert(0, new Tuple<object, string>(null, LOC("no value")));
		}

		_selectedOption = null;
		var valueJson = JsonSerializer.Serialize(_value);
		var optionIndex = _selectOptionsList.FindIndex(x => JsonSerializer.Serialize(x.Item1) == valueJson);
		if (optionIndex > -1)
		{
			_selectedOption = _selectOptionsList[optionIndex];
		}
		else if (_value is not null)
		{
			_selectOptionsList.Insert(0, new Tuple<object, string>(_value, _value.ToString()));
			_selectedOption = _selectOptionsList[0];
		}
	}

	private void _initStorageKeys()
	{
		_storageKey = this.GetType().Name + "_" + Context.SpaceViewColumnId;
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
}

public enum TfTextSelectColumnComponentOptionsSourceType
{
	[Description("manually set")]
	ManuallySet = 0,
	[Description("space data")]
	SpaceData = 1,
}