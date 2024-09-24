namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Long Integer Edit")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.LongIntegerEditColumnComponent.TfLongIntegerEditColumnComponent", "WebVella.Tefter")]
public partial class TfLongIntegerEditColumnComponent : TfBaseViewColumn<TfLongIntegerEditColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfLongIntegerEditColumnComponent()
	{
	}


	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfLongIntegerEditColumnComponent(TfComponentContext context)
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
	private long? _value = null;
	private string _valueInputId = "input-" + Guid.NewGuid();
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataObjectByAlias<long>(_valueAlias, null);
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

	private void _initValues()
	{
		_value = GetDataObjectByAlias<long>(_valueAlias, null);
	}
}

public class TfLongIntegerEditColumnComponentOptions
{
	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }
}