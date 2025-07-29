namespace WebVella.Tefter.UI.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponents.DateOnlyDisplayColumnComponent.TucDateOnlyDisplayColumnComponent", "WebVella.Tefter")]
public partial class TucDateOnlyDisplayColumnComponent : TucBaseViewColumn<TucDateOnlyDisplayColumnComponentOptions>
{
	public const string ID = "6ee59177-2aad-4c90-a5b9-702b91ff358d";
	public const string NAME = "Date Display";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";
	public const string VALUE_ALIAS = "Value";

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TucDateOnlyDisplayColumnComponent()
	{
	}
	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TucDateOnlyDisplayColumnComponent(TfSpaceViewColumnScreenRegionContext context)
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
		new Guid(TfDateOnlyViewColumnType.ID),
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnTypeAddon that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private List<DateOnly?> _value = new();
	private string _defaultFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

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
		//dateonly is not generally supported so we return datetime
		_initValues();
		if (_value.Count == 0)
		{
			return;
		}
		else if (_value.Count == 1)
		{
			if (_value[0] is null) return;
			excelCell.SetValue(XLCellValue.FromObject(((DateOnly?)_value[0]).ToDateTime()));
		}
		else
		{
			var valuesList = new List<string>();
			var format = !String.IsNullOrWhiteSpace(componentOptions.Format) ? componentOptions.Format : _defaultFormat;
			foreach (var item in _value)
			{
				if (item is null)
				{
					valuesList.Add(TfConstants.ExcelNullWord);
					continue;
				}
				valuesList.Add(item.Value.ToString(format));
			}
			excelCell.SetValue(XLCellValue.FromObject(String.Join(", ", valuesList)));
		}
	}
	#endregion

	#region << Private logic >>
	private void _initValues()
	{
		if(RegionContext.Mode != TfComponentPresentationMode.Display) return;
		_value = new();
		TfDataColumn column = GetColumnByAlias(VALUE_ALIAS);
		if (column is null)
			throw new Exception("Column not found");
		object columnData = GetColumnDataByAlias(VALUE_ALIAS);
		if (columnData is null)
		{
			_value.Add(null);
			return;
		}
		if (column.IsJoinColumn)
		{
			if (columnData.GetType().ImplementsInterface(typeof(IList)))
			{
				foreach (var joinValue in columnData as IList)
					_value.Add((DateOnly?)joinValue);
			}
		}
		else
			_value.Add((DateOnly?)columnData);
	}
	#endregion
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TucDateOnlyDisplayColumnComponentOptions
{
	[JsonPropertyName("Format")]
	public string Format { get; set; }
}