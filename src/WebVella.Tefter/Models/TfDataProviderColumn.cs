using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;


[DboCacheModel]
[TfDboModel("tf_data_provider_column")]
public class TfDataProviderColumn
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[Required]
	[TfDboModelProperty("source_name")]
	public string SourceName { get; set; }

	[TfDboModelProperty("source_type")]
	public string SourceType { get; set; }

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; }

	[Required]
	[TfDboModelProperty("db_name")]
	public string DbName { get; set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDatabaseColumnType>))]
	[TfDboModelProperty("db_type")]
	public TfDatabaseColumnType DbType { get; set; }

	[TfDboModelProperty("default_value")]
	public string DefaultValue { get; set; }

	[TfDboModelProperty("auto_default_value")]
	public bool AutoDefaultValue { get; set; }

	[TfDboModelProperty("is_nullable")]
	public bool IsNullable { get; set; }

	[TfDboModelProperty("is_unique")]
	public bool IsUnique { get; set; }


	[TfDboModelProperty("is_sortable")]
	public bool IsSortable { get; set; }

	[TfDboModelProperty("is_searchable")]
	public bool IsSearchable { get; set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDataProviderColumnSearchType>))]
	[TfDboModelProperty("preferred_search_type")]
	public TfDataProviderColumnSearchType PreferredSearchType { get; set; }

	[TfDboModelProperty("include_in_table_search")]
	public bool IncludeInTableSearch { get; set; }

	public void FixProviderPrefix(string dpPrefix)
	{
		if (!DbName.StartsWith(dpPrefix))
			DbName = dpPrefix + DbName;
	}
}
