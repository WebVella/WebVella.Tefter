namespace WebVella.Tefter;

public record TfColumnSort
{
	public List<string> DatabaseRequirementNames { get; init; }
	public Type DefaultComponentType { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
}

