namespace WebVella.Tefter;

//
// Summary:
//     Creates an Microsoft.Extensions.Localization.IStringLocalizer.
//
// Parameters:
//   baseName:
//     The base name of the resource to load strings from.
//
//   location:
//     The location to load resources from.
//
// Returns:
//     The Microsoft.Extensions.Localization.IStringLocalizer.

/// <summary>
/// Create a custom path to a localization resource of a component. Usefull when component namespace differes from folder structure
// Parameters:
//   baseName:
//     The base name of the resource to load strings from. Eg. "WebVella.Tefter.Web.ViewColumns.GuidViewColumn.TfGuidViewColumn"
//
//   location:
//     The location to load resources from.Eg. "WebVella.Tefter"
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class LocalizationResourceAttribute : Attribute
{
	public string BaseName { get; private set; }
	public string Location { get; private set; }
	public LocalizationResourceAttribute(string baseName, string location)
	{
		this.BaseName = baseName;
		this.Location = location;
	}
}
