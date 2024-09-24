namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Boolean Edit")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.BooleanEditColumnComponent.TfBooleanEditColumnComponent", "WebVella.Tefter")]
public partial class TfBooleanEditColumnComponent : TfBaseViewColumn<TfBooleanEditColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfBooleanEditColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfBooleanEditColumnComponent(TfComponentContext context)
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
	private bool _isThreeState = false;
	private bool _value = false;
	private bool? _state = false;

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
		}
	}


	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataObjectByAlias<bool>(_valueAlias, null);
	}

	private async Task _onValueChange()
	{
		if (options.ChangeRequiresConfirmation)
		{
			var message = options.ChangeConfirmationMessage;
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
			bool? value = _value;
			if (_isThreeState && _state is null) value = null;
			await OnRowColumnChangedByAlias(_valueAlias, _state);
			ToastService.ShowSuccess(LOC("change applied"));
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

	private void _initValues()
	{
		var column = GetColumnInfoByAlias(_valueAlias);
		if (column is not null)
		{
			_isThreeState = column.IsNullable;
			var value = GetDataObjectByAlias<bool>(_valueAlias, null);
			_value = value is null ? false : value.Value;
			_state = value;
		}
	}

	private string _getLabel()
	{
		if (!options.ShowLabel) return null;

		if (_isThreeState && _state is null)
		{
			if (!string.IsNullOrWhiteSpace(options.NullLabel)) return options.NullLabel;
			return "null";
		}
		else if (_value)
		{
			if (!string.IsNullOrWhiteSpace(options.TrueLabel)) return options.TrueLabel;
			return "true";
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(options.FalseLabel)) return options.FalseLabel;
			return "false";
		}
	}
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfBooleanEditColumnComponentOptions
{

	[JsonPropertyName("ShowLabel")]
	public bool ShowLabel { get; set; } = true;

	[JsonPropertyName("TrueLabel")]
	public string TrueLabel { get; set; }

	[JsonPropertyName("FalseLabel")]
	public string FalseLabel { get; set; }

	[JsonPropertyName("NullLabel")]
	public string NullLabel { get; set; }

	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }

}