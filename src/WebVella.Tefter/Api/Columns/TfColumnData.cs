namespace WebVella.Tefter;

public record TfColumnData
{
	public List<TfColumnColumnDatabaseRequirement> DatabaseRequirements { get; init; }
	public Type DefaultComponentType { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
}

