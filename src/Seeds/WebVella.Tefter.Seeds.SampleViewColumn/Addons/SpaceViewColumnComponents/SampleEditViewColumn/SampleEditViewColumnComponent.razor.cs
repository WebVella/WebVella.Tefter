using Microsoft.JSInterop;

namespace WebVella.Tefter.Seeds.Addons;

public partial class SampleEditViewColumnComponent : ComponentBase,ITfSpaceViewColumnComponentAddon
{
	public const string ID = "37d84705-57ac-4894-aa45-6244ecb9426a";
	public const string NAME = "Sample Text Edit";
	public const string DESCRIPTION = "sample component for edit text";
	public const string FLUENT_ICON_NAME = "DocumentText";
	public const string VALUE_ALIAS = "Value";

	[Inject] protected IJSRuntime JSRuntime { get; set; }

	#region << Properties >>
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<Guid> SupportedColumnTypes { get; init; } = new List<Guid> {
		new Guid(SampleViewColumnType.ID)
	};
	public TfSpaceViewColumnScreenRegionContext RegionContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public EventCallback<string> OptionsChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public EventCallback<TfDataTable> RowChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnTypeAddon that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
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
	protected async Task OnParametersSetAsync()
	{
		if (RegionContext.Hash != _renderedHash)
		{
			_initValues();
			_renderedHash = RegionContext.Hash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public void ProcessExcelCell(IServiceProvider serviceProvider,IXLCell excelCell)
	{
		var columnStringValue = string.Empty; //Get and set the column value here
		excelCell.SetValue(XLCellValue.FromObject(columnStringValue));
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
		//Do something on value changed
	}

	private void _initValues()
	{
		var columnDataString = String.Empty;//Get and set the column value here
		_value = columnDataString;
	}
	#endregion
}
