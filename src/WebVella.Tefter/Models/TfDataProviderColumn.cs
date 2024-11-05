using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;


[DboCacheModel]
[TfDboModel("data_provider_column")]
public class TfDataProviderColumn
{
	[TfDboModelProperty("id")]
	public Guid Id { get; internal set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; internal set; }

	[Required]
	[TfDboModelProperty("source_name")]
	public string SourceName { get; internal set; }

	[TfDboModelProperty("source_type")]
	public string SourceType { get; internal set; }

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("created_on")]
	public DateTime CreatedOn { get; internal set; }

	[Required]
	[TfDboModelProperty("db_name")]
	public string DbName { get; internal set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDatabaseColumnType>))]
	[TfDboModelProperty("db_type")]
	public TfDatabaseColumnType DbType { get; internal set; }

	[TfDboModelProperty("default_value")]
	public string DefaultValue { get; internal set; }

	[TfDboModelProperty("auto_default_value")]
	public bool AutoDefaultValue { get; internal set; }

	[TfDboModelProperty("is_nullable")]
	public bool IsNullable { get; internal set; }

	[TfDboModelProperty("is_unique")]
	public bool IsUnique { get; internal set; }


	[TfDboModelProperty("is_sortable")]
	public bool IsSortable { get; internal set; }

	[TfDboModelProperty("is_searchable")]
	public bool IsSearchable { get; internal set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDataProviderColumnSearchType>))]
	[TfDboModelProperty("preferred_search_type")]
	public TfDataProviderColumnSearchType PreferredSearchType { get; internal set; }

	[TfDboModelProperty("include_in_table_search")]
	public bool IncludeInTableSearch { get; internal set; }
}
