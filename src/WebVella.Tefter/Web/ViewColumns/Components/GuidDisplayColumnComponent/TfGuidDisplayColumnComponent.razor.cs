﻿namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter GUID")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.GuidDisplayColumnComponent.TfGuidDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfGuidDisplayColumnComponent : TfBaseViewColumn<TfGuidDisplayColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfGuidDisplayColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfGuidDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataObjectByAlias<Guid>("Value");
	}
}
/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfGuidDisplayColumnComponentOptions
{

}