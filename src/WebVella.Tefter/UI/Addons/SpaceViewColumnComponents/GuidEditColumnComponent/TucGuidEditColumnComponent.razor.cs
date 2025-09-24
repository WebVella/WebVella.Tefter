using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.UI.Addons;

public partial class TucGuidEditColumnComponent : TucBaseViewColumn<TucGuidEditColumnComponentOptions>
{
	public const string ID = "7b1a4f3e-500b-4b97-8833-dfe4e3ade1fb";
	public const string NAME = "GUID Edit";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";
	public const string VALUE_ALIAS = "Value";

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TucGuidEditColumnComponent()
	{
	}


	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TucGuidEditColumnComponent(TfSpaceViewColumnScreenRegionContext context)
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
		new Guid(TfGuidViewColumnType.ID),
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnTypeAddon that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private Guid? _value = null;
	private string _valueString = null;
	private string _valueInputId = "input-" + Guid.NewGuid();

	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private string _renderedHash = null;
	#endregion

	#region << Lifecycle >>
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
			_initValues();
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
		var column = GetColumnByAlias(VALUE_ALIAS);
		if (column == null) return;
		object? columnData = GetColumnData(column);
		if (columnData is not null && columnData is not Guid)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports Guid.");
		excelCell.SetValue(XLCellValue.FromObject((Guid?)columnData));
	}

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override string? GetValueAsString(IServiceProvider serviceProvider)
	{
		var column = GetColumnByAlias(VALUE_ALIAS);
		if (column == null) return null;
		object? columnData = GetColumnData(column);
		if (columnData is not null && columnData is not Guid)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports Guid.");

		if(columnData is null) return null;
		return ((Guid)columnData).ToString();
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
		Guid? value = null;
		if (!String.IsNullOrWhiteSpace(_valueString))
		{
			if (Guid.TryParse(_valueString, out Guid outGuid))
			{
				value = outGuid;
			}
			else
			{
				ToastService.ShowError(LOC("Invalid GUID format"));
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
			}
			;
		}

		try
		{
			await OnRowColumnChangedByAlias(VALUE_ALIAS, value);
			ToastService.ShowSuccess(LOC("change applied"));
			await JSRuntime.InvokeAsync<string>("Tefter.blurElementById", _valueInputId);
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
			await _resetValue();
		}
	}

	private void _initValues()
	{
		if(RegionContext.Mode != TfComponentPresentationMode.Display) return;
		TfDataColumn? column = GetColumnByAlias(VALUE_ALIAS);
		if (column is null)
			throw new Exception("Column not found");
		if (column.IsJoinColumn)
			throw new Exception("Joined data cannot be edited");

		object? columnData = GetColumnData(column);
		if (columnData is not null && columnData is not Guid)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports Guid.");
		_value = (Guid?)columnData;
		_valueString = _value?.ToString();
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

public class TucGuidEditColumnComponentOptions
{
	[JsonPropertyName("ChangeRequiresConfirmation")]
	public bool ChangeRequiresConfirmation { get; set; } = false;

	[JsonPropertyName("ChangeConfirmationMessage")]
	public string ChangeConfirmationMessage { get; set; }
}