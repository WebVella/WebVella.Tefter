namespace WebVella.Tefter.Assets.Addons;

public class TfFolderAssetsCountViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();


	public TfFolderAssetsCountViewColumnType()
	{

		Id = new Guid(AssetsConstants.ASSETS_ADDONS_SPACE_VIEW_COLUMN_TYPES_FOLDER_ASSET_COUNT_ID);

		Name = "Assets Folder Count";

		Description = "displays related files count";

		FluentIconName = "DocumentCopy";

		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
		};

		FilterAliases = new List<string>() { };

		SortAliases = new List<string> { };

		DefaultComponentId = new Guid(AssetsConstants.ASSETS_ADDONS_SPACE_VIEW_COLUMN_COMPONENTS_FOLDER_ASSET_COUNT_ID);
	}
}

