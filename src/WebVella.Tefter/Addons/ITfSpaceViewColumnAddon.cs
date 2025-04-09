namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnAddon : ITfAddon
{
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; set; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; }
}

public record TfSpaceViewColumnAddonDataMapping
{
	public string Alias { get; init; }
	public string Description { get; init; }
	public List<TfDatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
}


public class TfSpaceViewColumnAddonMeta
{
	public ITfSpaceViewColumnAddon Instance { get; init; }
}
