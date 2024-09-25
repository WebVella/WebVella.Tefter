﻿namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Time Display")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.TimeDisplayColumnComponent.TfTimeDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfTimeDisplayColumnComponent : TfBaseViewColumn<TfTimeDisplayColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfTimeDisplayColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTimeDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}

	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";

	private string _defaultFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + " "
	+ Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern;

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataObjectByAlias<DateTime>(_valueAlias); 
	}

}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfTimeDisplayColumnComponentOptions
{
	[JsonPropertyName("Format")]
	public string Format { get; set; }

}