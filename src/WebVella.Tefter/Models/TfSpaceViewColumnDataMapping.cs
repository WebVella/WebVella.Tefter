namespace WebVella.Tefter.Models;

public record TfSpaceViewColumnDataMapping
{
	public string Alias { get; init; }
	public string Description { get; init; }
	public List<DatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
}
