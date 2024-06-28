using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter;


[DboCacheModel]
[DboModel("data_provider_column")]
public class TfDataProviderColumn
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[Required]
	[DboModelProperty("source_name")]
	public string SourceName { get; set; }

	[DboModelProperty("source_type")]
	public string SourceType { get; set; }

	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	[DboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; }

	[Required]
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

	[DboModelProperty("is_unique")]
	public bool IsUnique { get; set; }


	[DboModelProperty("is_sortable")]
	public bool IsSortable { get; set; }

	[DboModelProperty("is_searchable")]
	public bool IsSearchable { get; set; }

	[DboTypeConverter(typeof(EnumPropertyConverter<TfDataProviderColumnSearchType>))]
	[DboModelProperty("preferred_search_type")]
	public TfDataProviderColumnSearchType PreferredSearchType { get; set; }

	[DboModelProperty("include_in_table_search")]
	public bool IncludeInTableSearch { get; set; }
}
