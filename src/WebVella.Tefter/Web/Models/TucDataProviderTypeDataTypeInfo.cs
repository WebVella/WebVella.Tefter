namespace WebVella.Tefter.Web.Models;
public record TucDataProviderTypeDataTypeInfo
{
	public string Name { get; init; }
	public List<TucDatabaseColumnTypeInfo> SupportedDatabaseColumnTypes { get; init; } = new();
}
