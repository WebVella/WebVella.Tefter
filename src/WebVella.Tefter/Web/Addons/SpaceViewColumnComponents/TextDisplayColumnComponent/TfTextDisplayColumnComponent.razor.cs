namespace WebVella.Tefter.Web.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponents.TextDisplayColumnComponent.TfTextDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfTextDisplayColumnComponent : TucBaseViewColumn<TfTextDisplayColumnComponentOptions>
{
	public const string ID = "f7f6a912-f670-4275-8794-13a483cac2c9";
	public const string NAME = "Text Display";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";
	public const string VALUE_ALIAS = "Value";

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfTextDisplayColumnComponent() { }

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTextDisplayColumnComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid(ID);
	public override string Name { get; init; } = NAME;
	public override string Description { get; init; } = DESCRIPTION;
	public override string FluentIconName { get; init; } = FLUENT_ICON_NAME;

	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
		new Guid(TfTextViewColumnType.ID),
		new Guid(TfBooleanViewColumnType.ID),
		new Guid(TfDateOnlyViewColumnType.ID),
		new Guid(TfDateTimeViewColumnType.ID),
		new Guid(TfGuidViewColumnType.ID),
		new Guid(TfIntegerViewColumnType.ID),
		new Guid(TfLongIntegerViewColumnType.ID),
		new Guid(TfShortIntegerViewColumnType.ID),
		new Guid(TfNumberViewColumnType.ID),
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnTypeAddon that renders this component
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
	public override void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell)
	{
		object columnData = GetColumnDataByAlias(VALUE_ALIAS);
		var columnDataString = String.Empty;
		if (columnData is not null)
		{
			columnDataString = columnData.ToString();
		}
		excelCell.SetValue(XLCellValue.FromObject(columnDataString));
	}
	#endregion

	#region << Private logic >>
	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(VALUE_ALIAS);
		var columnDataString = String.Empty;
		if (columnData is not null)
		{
			if (columnData.GetType().ImplementsInterface(typeof(IList)))
			{
				var results = new List<string>();
				foreach (var item in columnData as IList)
				{
					results.Add(item.ToString());
				}
				columnDataString = String.Join(", ", results);
			}
			else
				columnDataString = columnData.ToString();
		}
		_value = columnDataString;
	}
	#endregion
}

public class TfTextDisplayColumnComponentOptions { }