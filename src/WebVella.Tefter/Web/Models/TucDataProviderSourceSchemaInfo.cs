namespace WebVella.Tefter.Models;

public class TucDataProviderSourceSchemaInfo
{
	public Dictionary<string, TfDatabaseColumnType> SourceColumnDatabaseType { get; set; } = new();
	public Dictionary<TfDatabaseColumnType,string> DatabaseTypeToSourceType { get; set; } = new();

	public TucDataProviderSourceSchemaInfo()
	{
	}
	public TucDataProviderSourceSchemaInfo(TfDataProviderSourceSchemaInfo model)
	{
		SourceColumnDatabaseType = model.SourceColumnDatabaseType;
		DatabaseTypeToSourceType = model.DatabaseTypeToSourceType;
	}
}
