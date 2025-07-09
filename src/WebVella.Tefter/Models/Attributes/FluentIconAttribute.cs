namespace WebVella.Tefter.Models;

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
//     The base name of the resource to load strings from. Eg. "WebVella.Tefter.UI.Addons.GuidViewColumn.TfGuidViewColumn"
//
//   location:
//     The location to load resources from.Eg. "WebVella.Tefter"
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class FluentIconAttribute : Attribute
{
	public string Name { get; private set; }
	public FluentIconAttribute(string name)
	{
		this.Name = name;
	}
}
