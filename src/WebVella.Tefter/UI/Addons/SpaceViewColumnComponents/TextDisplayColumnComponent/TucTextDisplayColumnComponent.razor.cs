using System.ComponentModel.Design;

namespace WebVella.Tefter.UI.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponents.TextDisplayColumnComponent.TucTextDisplayColumnComponent", "WebVella.Tefter")]
public partial class TucTextDisplayColumnComponent : TucBaseViewColumn<TucTextDisplayColumnComponentOptions>
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
	public TucTextDisplayColumnComponent() { }

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TucTextDisplayColumnComponent(TfSpaceViewColumnScreenRegionContext context)
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
	//when only single value is provided by the DataTable
	private List<string> _value = new();

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
		_initValues();
		excelCell.SetValue(XLCellValue.FromObject(String.Join(", ", _value)));
	}
	#endregion

	#region << Private logic >>
	private void _initValues()
	{
		if(RegionContext.Mode != TfComponentPresentationMode.Display) return;
		_value = new();
		TfDataColumn? column = GetColumnByAlias(VALUE_ALIAS);
		if (column is null)
			throw new Exception("Column not found");
		object? columnData = GetColumnData(column);
		if (columnData is null)
		{
			_value.Add(String.Empty);
			return;
		}
		if (column.IsJoinColumn)
		{
			if (columnData.GetType().ImplementsInterface(typeof(IList)))
			{
				foreach (var joinValue in columnData as IList)
					_value.Add(joinValue?.ToString());
			}
		}
		else
			_value.Add(columnData.ToString());
	}
	#endregion
}

public class TucTextDisplayColumnComponentOptions { }