﻿namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Email Display")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.EmailDisplayColumnComponent.TfEmailDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfEmailDisplayColumnComponent : TucBaseViewColumn<TfEmailDisplayColumnComponentOptions>
{
	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfEmailDisplayColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfEmailDisplayColumnComponent(TfSpaceViewColumnScreenRegion context)
	{
		Context = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid("c07af5f0-0a25-4786-8043-375f22cde7bd");
	public override List<Type> SupportedColumnTypes { get; init; } = new List<Type>{
		typeof(TfTextViewColumnType)
	};
	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";
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
		if (Context.Hash != _renderedHash)
		{
			_initValues();
			_renderedHash = Context.Hash;
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
		if (columnData is not null && columnData is not string) 
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports string.");
		excelCell.SetValue(XLCellValue.FromObject((string)columnData));
	}
	#endregion

	#region << Private logic >>
	private void _initValues()
	{
		object columnData = GetColumnDataByAlias(_valueAlias);
		if (columnData is not null && columnData is not string) 
			throw new Exception($"Not supported data type of '{columnData.GetType()}'. Supports string.");
		_value = (string)columnData;
	}
	#endregion
}

public class TfEmailDisplayColumnComponentOptions { }