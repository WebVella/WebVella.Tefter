namespace WebVella.Tefter.Models;

public class TfDataProviderSourceSchemaInfo
{
	public Dictionary<string, string> SourceColumnDefaultSourceType { get; set; } = new();
	public Dictionary<string, TfDatabaseColumnType> SourceColumnDefaultDbType { get; set; } = new();
	public Dictionary<string, string> SourceColumnDefaultValue { get; set; } = new();
	public Dictionary<string, List<TfDatabaseColumnType>> SourceTypeSupportedDbTypes { get; set; } = new();
	public List<string> SynchPrimaryKeyColumns { get; set; } = new();
}
