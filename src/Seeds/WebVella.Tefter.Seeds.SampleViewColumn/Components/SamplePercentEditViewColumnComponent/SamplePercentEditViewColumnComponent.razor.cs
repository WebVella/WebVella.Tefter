﻿namespace WebVella.Tefter.Seeds.SampleViewColumn.Components;

[Description("Integer Edit")]
public partial class SamplePercentEditViewColumnComponent : TucBaseViewColumn<TfIntegerEditColumnComponentOptions>
{
	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public SamplePercentEditViewColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public SamplePercentEditViewColumnComponent(TucViewColumnComponentContext context)
	{
		Context = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid("b84671e2-44ca-4746-93f6-c0d81840b0d1");
	public override List<Type> SupportedColumnTypes { get; init; } = new List<Type>{
		typeof(SamplePercentViewColumnType)
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private decimal? _value = null;
	private string _valueInputId = "input-" + Guid.NewGuid();
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

	#region << Private logic >>
	/// <summary>
	/// Because of the wheel functionality, user can initiate changes very quickly
	/// This throttle will submit only after 1 second of inactivity
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	private async Task _valueChanged(decimal? value)
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
		try
		{
			await OnRowColumnChangedByAlias(_valueAlias, _value);
			ToastService.ShowSuccess("change applied");
			await JSRuntime.InvokeAsync<string>("Tefter.blurElementById", new object[] { _valueInputId });
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
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && !(columnData is int || columnData is short || columnData is long || columnData is decimal))
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports only number based types.");
		
		_value = (decimal?)columnData;
	}
	#endregion
}

public class TfIntegerEditColumnComponentOptions
{
}