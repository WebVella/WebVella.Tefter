namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnTypeAddon : ITfAddon
{
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultDisplayComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; }
	public Guid? DefaultEditComponentId { get; init; }
}

public record TfSpaceViewColumnAddonDataMapping
{
	public string Alias { get; init; }
	public string Description { get; init; }
	public List<TfDatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
}


public class TfSpaceViewColumnTypeAddonMeta
{
	public ITfSpaceViewColumnTypeAddon Instance { get; init; }
}
