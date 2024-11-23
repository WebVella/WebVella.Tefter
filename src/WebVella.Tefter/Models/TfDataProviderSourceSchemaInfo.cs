namespace WebVella.Tefter.Models;

public class TfDataProviderSourceSchemaInfo
{
	public Dictionary<string, TfDatabaseColumnType> SourceColumnDatabaseType { get; set; } = new();
	public Dictionary<TfDatabaseColumnType,string> DatabaseTypeToSourceType { get; set; } = new();
}
