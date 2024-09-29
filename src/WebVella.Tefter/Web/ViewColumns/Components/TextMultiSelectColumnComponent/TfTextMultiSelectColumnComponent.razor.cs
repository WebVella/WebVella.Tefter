namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Text MultiSelect")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.TextMultiSelectColumnComponent.TfTextMultiSelectColumnComponent", "WebVella.Tefter")]
public partial class TfTextMultiSelectColumnComponent : TfBaseViewColumn<TfTextMultiSelectColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfTextMultiSelectColumnComponent()
	{
	}


	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTextMultiSelectColumnComponent(TfComponentContext context)
	{
		Context = context;
	}

	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private List<object> _value = new List<object>();
	private string _valueInputId = "input-" + Guid.NewGuid();
	private List<Tuple<object, string>> _options = new();
	private List<Tuple<object, string>> _selectedOptions = new();
	private bool _open = false;
	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private Guid? _renderedHash = null;

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

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataStringByAlias(_valueAlias);
	}

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
			if(_value is not null && _value.Count == 0) _value = null;
			string stringValue = null;
			if(_value is not null) stringValue = JsonSerializer.Serialize(_value);

			await OnRowColumnChangedByAlias(_valueAlias, stringValue);
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
	private async Task _optionChanged(IEnumerable<Tuple<object, string>> options)
	{
		if (options is null && (_value is null || _value.Count == 0)) return;
		if (_value is null) _value = new List<object>();
		//var optionIndex = _value.FindIndex(x => x == option.Item1);
		//if (optionIndex > -1) _value.RemoveAt(optionIndex);
		//else _value.Add(option.Item1);
		//await _valueChanged();
	}
	private void _initValues()
	{
		_value = GetDataObjectFromJsonByAlias<List<object>>(_valueAlias, new List<object>());

		_options.Clear();
		_selectedOptions.Clear();
		if (!String.IsNullOrWhiteSpace(componentOptions.OptionsString))
		{
			var rows = componentOptions.OptionsString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
			foreach (var row in rows)
			{
				var items = row.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
				if (items.Count == 1)
				{
					_options.Add(new Tuple<object, string>(items[0], items[0]));
				}
				else if (items.Count > 1)
				{
					_options.Add(new Tuple<object, string>(items[0], items[1]));
				}

			}
		}


		if (_value is null || _value.Count == 0)
		{
			_selectedOptions = new();
			return;
		}
		foreach (var value in _value)
		{
			if (_options.Any(x => x.Item1 == value))
			{
				_selectedOptions.Add(_options.First(x => x.Item1 == value));
			}
			else
			{
				_options.Insert(0, new Tuple<object, string>(value, value.ToString()));
				_selectedOptions.Add(_options[0]);
			}
		}


	}
}

public class TfTextMultiSelectColumnComponentOptions
{
	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }

	[JsonPropertyName("OptionsString")]
	public string OptionsString { get; set; }
}