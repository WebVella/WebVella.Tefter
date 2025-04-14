
namespace WebVella.Tefter.Seeds.Addons;

public partial class SampleDisplayViewColumnComponent : ComponentBase, ITfSpaceViewColumnComponentAddon
{
	public const string ID = "40856b22-4511-47fe-878e-4248173457e4";
	public const string NAME = "Sample Text Display";
	public const string DESCRIPTION = "sample component for displaying text";
	public const string FLUENT_ICON_NAME = "DocumentText";
	public const string VALUE_ALIAS = "Value";

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
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _value = null;

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

	private void _initValues()
	{
		var columnDataString = String.Empty;//Get and set the column value here
		_value = columnDataString;
	}
}