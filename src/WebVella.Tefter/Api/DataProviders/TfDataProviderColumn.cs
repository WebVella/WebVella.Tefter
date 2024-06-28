using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter;


[DboCacheModel]
[DboModel("data_provider_column")]
public class TfDataProviderColumn
{
	[DboModelProperty("id")]
	public Guid Id { get; internal set; }

	[DboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; internal set; }

	[Required]
	[DboModelProperty("source_name")]
	public string SourceName { get; internal set; }

	[DboModelProperty("source_type")]
	public string SourceType { get; internal set; }

	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	[DboModelProperty("created_on")]
	public DateTime CreatedOn { get; internal set; }

	[Required]
	[DboModelProperty("db_name")]
	public string DbName { get; internal set; }

	[DboTypeConverter(typeof(EnumPropertyConverter<DatabaseColumnType>))]
	[DboModelProperty("db_type")]
	public DatabaseColumnType DbType { get; internal set; }

	[DboModelProperty("default_value")]
	public string DefaultValue { get; internal set; }

	[DboModelProperty("auto_default_value")]
	public bool AutoDefaultValue { get; internal set; }

	[DboModelProperty("is_nullable")]
	public bool IsNullable { get; internal set; }

	[DboModelProperty("is_unique")]
	public bool IsUnique { get; internal set; }


	[DboModelProperty("is_sortable")]
	public bool IsSortable { get; internal set; }

	[DboModelProperty("is_searchable")]
	public bool IsSearchable { get; internal set; }

	[DboTypeConverter(typeof(EnumPropertyConverter<TfDataProviderColumnSearchType>))]
	[DboModelProperty("preferred_search_type")]
	public TfDataProviderColumnSearchType PreferredSearchType { get; internal set; }

	[DboModelProperty("include_in_table_search")]
	public bool IncludeInTableSearch { get; internal set; }
}
