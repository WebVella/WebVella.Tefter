namespace WebVella.Tefter;

internal class TfDataProviderColumn
{
	public Guid Id { get; set; }
	public bool IsRowKey { get; set; }
	public string SourceName { get; set; }
	public string SourceType { get; set; }
	public int Index { get; set; }
	public string DbName { get; set; }
	public DatabaseColumnType DbType { get; set; }
	public string DefaultValue { get; set; }
	public bool AutoDefaultValue { get; set; }
	public bool IsNullable { get; set; }
	public bool IsSortable { get; set; }
	public bool IsSearchable { get; set; }
	public TfDataProviderColumnSearchType PreferedSearchType { get; set; }
	public bool IncludeInTableSearch { get; set; }
	public bool IsSystem { get; set; }
}
