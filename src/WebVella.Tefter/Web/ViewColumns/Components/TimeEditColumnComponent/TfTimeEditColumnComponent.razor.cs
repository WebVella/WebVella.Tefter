namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Time Edit")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.TimeEditColumnComponent.TfTimeEditColumnComponent", "WebVella.Tefter")]
public partial class TfTimeEditColumnComponent : TucBaseViewColumn<TfTimeEditColumnComponentOptions>
{
	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfTimeEditColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTimeEditColumnComponent(TucViewColumnComponentContext context)
	{
		Context = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid("4af7582b-b2c9-411d-a121-2f1ff2be6ca2");
	public override List<Type> SupportedColumnTypes { get; init; } = new List<Type>{
		typeof(TfDateTimeViewColumnType)
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private DateTime? _value = null;
	private string _valueInputId = "input-" + Guid.NewGuid();
	private string _valueTimeInputId = "input-" + Guid.NewGuid();
	private CancellationTokenSource inputThrottleCancalationToken = new();
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
	public override void ProcessExcelCell(IServiceProvider serviceProvider,IXLCell excelCell)
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not DateTime)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports DateTime.");
		excelCell.SetValue(XLCellValue.FromObject((DateTime?)columnData));
	}
	#endregion

	#region << Private logic >>
	/// <summary>
	/// Because of the wheel functionality, user can initiate changes very quickly
	/// This throttle will submit only after 1 second of inactivity
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	private async Task _valueChanged(DateTime? value)
	{
		_value = value;
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(1000, inputThrottleCancalationToken.Token).ContinueWith(async (task) => { await InvokeAsync(_submitChange); }, inputThrottleCancalationToken.Token);
	}

	/// <summary>
	/// process the value change event from the components view
	/// by design if any kind of error occurs the old value should be set back
	/// so the user is notified that the change is aborted
	/// </summary>
	/// <returns></returns>
	private async Task _submitChange()
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
			await JSRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
			await InvokeAsync(StateHasChanged);
			await Task.Delay(10);
			//_initValues();
			await InvokeAsync(StateHasChanged);
		}
	}
	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not DateTime)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports DateTime.");
		_value = (DateTime?)columnData;
	}

	#endregion
}

public class TfTimeEditColumnComponentOptions
{
	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }
}