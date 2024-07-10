namespace WebVella.Tefter.UseCases.Models;
public record TucDataProviderTypeDataTypeInfo
{
	public string Name { get; init; }
	public List<TucDatabaseColumnTypeInfo> SupportedDatabaseColumnTypes { get; init; } = new();
}
