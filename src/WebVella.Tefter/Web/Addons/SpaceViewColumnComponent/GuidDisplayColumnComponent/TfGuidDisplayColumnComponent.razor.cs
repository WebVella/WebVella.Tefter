﻿namespace WebVella.Tefter.Web.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponent.GuidDisplayColumnComponent.TfGuidDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfGuidDisplayColumnComponent : TucBaseViewColumn<TfGuidDisplayColumnComponentOptions>
{
	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfGuidDisplayColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfGuidDisplayColumnComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid(TfConstants.TF_COLUMN_COMPONENT_DISPLAY_GUID_ID);
	public override string Name { get; init;} = "GUID Display";
	public override string Description { get; init;} = String.Empty;
	public override string FluentIconName { get; init;} = String.Empty;
	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
		new Guid(TfConstants.TF_GENERIC_GUID_COLUMN_TYPE_ID),
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
	private Guid? _value = null;

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
	public override void ProcessExcelCell(IServiceProvider serviceProvider,IXLCell excelCell)
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not Guid) 
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports Guid.");
		excelCell.SetValue(XLCellValue.FromObject((Guid?)columnData));
	}
	#endregion

	#region << Private logic >>
	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not Guid) 
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports Guid.");
		_value =  (Guid?)columnData;
	}
	#endregion
}
/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfGuidDisplayColumnComponentOptions
{

}