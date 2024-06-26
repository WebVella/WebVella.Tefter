namespace WebVella.Tefter;


[DboCacheModel]
[DboModel("data_provider_column")]
public class TfDataProviderColumn
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("is_row_key")]
	public bool IsRowKey { get; set; }

	[DboModelProperty("source_name")]
	public string SourceName { get; set; }

	[DboModelProperty("source_type")]
	public string SourceType { get; set; }


	[DboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[DboAutoIncrementModel]
	[DboModelProperty("index")]
	public int Index { get; set; }

	[DboModelProperty("db_name")]
	public string DbName { get; set; }

	[DboTypeConverter(typeof(EnumPropertyConverter<DatabaseColumnType>))]
	[DboModelProperty("db_type")]
	public DatabaseColumnType DbType { get; set; }

	[DboModelProperty("default_value")]
	public string DefaultValue { get; set; }

	[DboModelProperty("auto_default_value")]
	public bool AutoDefaultValue { get; set; }

	[DboModelProperty("is_nullable")]
	public bool IsNullable { get; set; }

	[DboModelProperty("is_sortable")]
	public bool IsSortable { get; set; }

	[DboModelProperty("is_searchable")]
	public bool IsSearchable { get; set; }

	[DboTypeConverter(typeof(EnumPropertyConverter<TfDataProviderColumnSearchType>))]
	[DboModelProperty("preferred_search_type")]
	public TfDataProviderColumnSearchType PreferredSearchType { get; set; }

	[DboModelProperty("include_table_search")]
	public bool IncludeInTableSearch { get; set; }

	[DboModelProperty("is_system")]
	public bool IsSystem { get; set; }
}
