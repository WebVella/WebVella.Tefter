namespace WebVella.Tefter;

public record TfColumnColumnDatabaseRequirement
{
	public string Name { get; init; }
	public string Description { get; init; }
	public List<DatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
}
