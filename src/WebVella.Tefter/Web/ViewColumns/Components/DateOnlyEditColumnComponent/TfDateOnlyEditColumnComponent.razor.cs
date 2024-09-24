namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Date Edit")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.DateOnlyEditColumnComponent.TfDateOnlyEditColumnComponent", "WebVella.Tefter")]
public partial class TfDateOnlyEditColumnComponent : TfBaseViewColumn<TfDateOnlyEditColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfDateOnlyEditColumnComponent()
	{
	}


	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfDateOnlyEditColumnComponent(TfComponentContext context)
	{
		Context = context;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initValues();
	}

	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private DateTime? _value = null;
	private string _valueInputId = "input-" + Guid.NewGuid();
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		//dateonly is not generally supported so we return datetime
		var dateOnly = GetDataObjectByAlias<DateOnly>(_valueAlias, null);
		if (dateOnly is null) return null;

		return dateOnly.Value.ToDateTime();
	}

	/// <summary>
	/// process the value change event from the components view
	/// by design if any kind of error occurs the old value should be set back
	/// so the user is notified that the change is aborted
	/// </summary>
	/// <returns></returns>
	private async Task _valueChanged()
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
			DateOnly? doValue = null;
			if(_value is not null) doValue = new DateOnly(_value.Value.Year,_value.Value.Month,_value.Value.Day);
			await OnRowColumnChangedByAlias(_valueAlias, doValue);
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

	private void _initValues()
	{
		var dateOnly = GetDataObjectByAlias<DateOnly>(_valueAlias, null);
		if (dateOnly is null) _value = null;

		_value = dateOnly.Value.ToDateTime();
	}
}

public class TfDateOnlyEditColumnComponentOptions
{
	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }
}