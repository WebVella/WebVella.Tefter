namespace WebVella.Tefter.Assets.Addons;

public class TfFolderAssetsCountViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "aafd5f8a-95d0-4f6b-8b43-c75a80316504";
	public const string NAME = "Assets Folder Count";
	public const string DESCRIPTION = "displays related files count";
	public const string FLUENT_ICON_NAME = "DocumentCopy";

	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; } = new();
	public Guid? DefaultComponentId { get; init; } = new Guid(TfFolderAssetsCountComponent.ID);
	public List<string> FilterAliases { get; init; } = new();
	public List<string> SortAliases { get; init; } = new();
	public List<Guid> SupportedComponents { get; set; } = new();
}

