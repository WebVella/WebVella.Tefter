﻿namespace WebVella.Tefter.Web.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponents.IntegerDisplayColumnComponent.TfIntegerDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfShortIntegerDisplayColumnComponent : TucBaseViewColumn<TfShortIntegerDisplayColumnComponentOptions>
{
	public const string ID = "02f6f53c-cbe9-42e6-92a1-9fc5c7efb229";
	public const string NAME = "Short Integer Display";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";
	public const string VALUE_ALIAS = "Value";

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfShortIntegerDisplayColumnComponent()
	{
	}
	
	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>	
	public TfShortIntegerDisplayColumnComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid(ID);
	public override string Name { get; init;} = NAME;
	public override string Description { get; init;} = DESCRIPTION;
	public override string FluentIconName { get; init;} = FLUENT_ICON_NAME;
	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
		new Guid(TfShortIntegerViewColumnType.ID),
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnTypeAddon that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private short? _value = null;

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
		object columnData = GetColumnDataByAlias(VALUE_ALIAS);
		if (columnData is not null && columnData is not short)
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports short.");
		excelCell.SetValue(XLCellValue.FromObject((short?)columnData));
	}
	#endregion

	#region << Private logic >>
	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(VALUE_ALIAS);
		if(columnData is not null && columnData is not short) 
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports short.");
		_value = (short?)columnData;
	}
	#endregion
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfShortIntegerDisplayColumnComponentOptions
{
	[JsonPropertyName("Format")]
	public string Format { get; set; }
}