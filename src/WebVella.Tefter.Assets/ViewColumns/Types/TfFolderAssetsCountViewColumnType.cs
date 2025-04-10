namespace WebVella.Tefter.Assets;

public class TfFolderAssetsCountViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; set; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();


	public TfFolderAssetsCountViewColumnType()
	{

		Id = new Guid("aafd5f8a-95d0-4f6b-8b43-c75a80316504");

		Name = "Assets Folder Count";

		Description = "displays related files count";

		FluentIconName = "DocumentCopy";

		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
		};

		FilterAliases = new List<string>() { };

		SortAliases = new List<string> { };

		DefaultComponentType = typeof(TfFolderAssetsCountComponent);
	}
}

