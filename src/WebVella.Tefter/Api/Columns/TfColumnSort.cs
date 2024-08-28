namespace WebVella.Tefter;

public record TfColumnSort
{
	//list from the requirement aliases which are already mapped in the data
	//to db_name
	public List<string> DatabaseRequirementNames { get; init; }
	public Type DefaultComponentType { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
}

