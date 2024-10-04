namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter URL Edit")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.UrlEditColumnComponent.TfUrlEditColumnComponent", "WebVella.Tefter")]
public partial class TfUrlEditColumnComponent : TucBaseViewColumn<TfUrlEditColumnComponentOptions>
{
	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfUrlEditColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfUrlEditColumnComponent(TucViewColumnComponentContext context)
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
	private string _value = null;
	private string _valueInputId = "input-" + Guid.NewGuid();

	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private Guid? _renderedHash = null;
	#endregion

	#region << Lifecycle >>
	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
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
	public override object GetData(IServiceProvider serviceProvider)
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not string)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports string.");
		return (string)columnData;
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
		if (!String.IsNullOrWhiteSpace(_value))
		{
			if (!TfConverters.IsValidURL(_value))
			{
				ToastService.ShowError(LOC("Invalid URL format"));
				await _resetValue();
				return;
			}
		}
		if (componentOptions.ChangeRequiresConfirmation)
		{
			var message = componentOptions.ChangeConfirmationMessage;
			if (String.IsNullOrWhiteSpace(message))
				message = LOC("Please confirm the data change!");

			if (!await JSRuntime.InvokeAsync<bool>("confirm", message))
			{
				await _resetValue();
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
			await _resetValue();
		}
	}

	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not string)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports string.");
		_value = (string)columnData;
	}

	private async Task _resetValue()
	{
		await InvokeAsync(StateHasChanged);
		await Task.Delay(10);
		_initValues();
		await InvokeAsync(StateHasChanged);
	}
	#endregion
}

public class TfUrlEditColumnComponentOptions
{
	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }
}