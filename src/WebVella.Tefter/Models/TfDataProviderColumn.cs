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

	[TfDboModelProperty("source_name")]
	public string? SourceName { get; set; }

	[TfDboModelProperty("source_type")]
	public string? SourceType { get; set; } = null;

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; }

	[Required]
	[TfDboModelProperty("db_name")]
	public string? DbName { get; set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDatabaseColumnType>))]
	[TfDboModelProperty("db_type")]
	public TfDatabaseColumnType DbType { get; set; } = TfDatabaseColumnType.Text;

	[TfDboModelProperty("default_value")]
	public string? DefaultValue { get; set; }

	[TfDboModelProperty("auto_default_value")]
	public bool AutoDefaultValue { get; set; } = true;

	[TfDboModelProperty("is_nullable")]
	public bool IsNullable { get; set; } = true;

	[TfDboModelProperty("is_unique")]
	public bool IsUnique { get; set; }


	[TfDboModelProperty("is_sortable")]
	public bool IsSortable { get; set; }

	[TfDboModelProperty("is_searchable")]
	public bool IsSearchable { get; set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDataProviderColumnSearchType>))]
	[TfDboModelProperty("preferred_search_type")]
	public TfDataProviderColumnSearchType PreferredSearchType { get; set; } = TfDataProviderColumnSearchType.Contains;

	[TfDboModelProperty("include_in_table_search")]
	public bool IncludeInTableSearch { get; set; }

	public void FixPrefix(string prefix)
	{
		if (!String.IsNullOrWhiteSpace(DbName) && !DbName.StartsWith(prefix))
			DbName = prefix + DbName;
	}
}
