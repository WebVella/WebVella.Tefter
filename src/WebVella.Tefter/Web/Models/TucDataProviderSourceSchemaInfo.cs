namespace WebVella.Tefter.Models;

public class TucDataProviderSourceSchemaInfo
{
	public Dictionary<string, string> SourceColumnDefaultSourceType { get; set; } = new();
	public Dictionary<string, TfDatabaseColumnType> SourceColumnDefaultDbType { get; set; } = new();
	public Dictionary<string, List<TfDatabaseColumnType>> SourceTypeSupportedDbTypes { get; set; } = new();

	public TucDataProviderSourceSchemaInfo()
	{
	}
	public TucDataProviderSourceSchemaInfo(TfDataProviderSourceSchemaInfo model)
	{
		SourceColumnDefaultSourceType = model.SourceColumnDefaultSourceType;
		SourceColumnDefaultDbType = model.SourceColumnDefaultDbType;
		SourceTypeSupportedDbTypes = model.SourceTypeSupportedDbTypes;
	}
}
