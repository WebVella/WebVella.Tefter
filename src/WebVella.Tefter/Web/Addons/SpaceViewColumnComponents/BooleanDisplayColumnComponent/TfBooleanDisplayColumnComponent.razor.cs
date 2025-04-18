﻿namespace WebVella.Tefter.Web.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[LocalizationResource("WebVella.Tefter.Web.Addons.SpaceViewColumnComponents.BooleanDisplayColumnComponent.TfBooleanDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfBooleanDisplayColumnComponent : TucBaseViewColumn<TfBooleanDisplayColumnComponentOptions>
{
	public const string ID = "a1119d49-aa69-4140-aaa3-de2b9d6a78bb";
	public const string NAME = "Boolean Display";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";
	public const string VALUE_ALIAS = "Value";

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfBooleanDisplayColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfBooleanDisplayColumnComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init;} = new Guid(ID);
	public override string Name { get; init;} = NAME;
	public override string Description { get; init;} = DESCRIPTION;
	public override string FluentIconName { get; init;} = FLUENT_ICON_NAME;
	public override List<Guid> SupportedColumnTypes { get; init;} = new List<Guid>{ 
		new Guid(TfBooleanViewColumnType.ID),
	};

	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnTypeAddon that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private bool? _value = null;

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
		if (columnData is not null && columnData is not bool) 
			throw new Exception($"Not supported data type of '{columnData.GetType()}'");

		bool? value = (bool?)columnData;
		if (value is null) return;

		//options are not inited yet as the component is not rendered
		var options = GetOptions();

		if (value.Value && !String.IsNullOrWhiteSpace(options.TrueLabel)) excelCell.SetValue(XLCellValue.FromObject(options.TrueLabel));
		else if (!value.Value && !String.IsNullOrWhiteSpace(options.FalseLabel)) excelCell.SetValue(XLCellValue.FromObject(options.FalseLabel));

		excelCell.SetValue(XLCellValue.FromObject(value));
	}
	#endregion

	#region << Private logic >>
	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(VALUE_ALIAS);
		if (columnData is not null && columnData is not bool) 
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports Boolean.");
		_value = (bool?)columnData;
	}
	#endregion
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfBooleanDisplayColumnComponentOptions
{
	[JsonPropertyName("TrueLabel")]
	public string TrueLabel { get; set; }

	[JsonPropertyName("TrueValueShowAsIcon")]
	public bool TrueValueShowAsIcon { get; set; }

	[JsonPropertyName("FalseLabel")]
	public string FalseLabel { get; set; }

	[JsonPropertyName("FalseValueShowAsIcon")]
	public bool FalseValueShowAsIcon { get; set; }

	[JsonPropertyName("NullLabel")]
	public string NullLabel { get; set; }

	[JsonPropertyName("NullValueShowAsIcon")]
	public bool NullValueShowAsIcon { get; set; }

}