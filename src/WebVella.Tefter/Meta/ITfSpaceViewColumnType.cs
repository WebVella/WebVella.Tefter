namespace WebVella.Tefter.Models;

public interface ITfSpaceViewColumnType
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string Icon { get; init; }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; }
}

public record TfSpaceViewColumnDataMapping
{
	public string Alias { get; init; }
	public string Description { get; init; }
	public List<DatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
}


public class TfSpaceViewColumnTypeMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public string Icon { get { return Instance.Icon; } }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get { return Instance.DataMapping; } }
	public Type DefaultComponentType { get { return Instance.DefaultComponentType; } }
	public List<Type> SupportedComponentTypes { get { return Instance.SupportedComponentTypes; } }
	public List<string> FilterAliases { get { return Instance.FilterAliases; } }
	public List<string> SortAliases { get { return Instance.SortAliases; } }
	public List<Guid> SupportedAddonTypes { get { return Instance.SupportedAddonTypes; } }
	public ITfSpaceViewColumnType Instance { get; init; }
}
