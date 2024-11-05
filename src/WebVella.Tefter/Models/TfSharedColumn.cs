namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("shared_column")]
public record TfSharedColumn
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("shared_key_db_name")]
	public string SharedKeyDbName { get; set; }

	[TfDboModelProperty("db_name")]
	public string DbName { get; set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDatabaseColumnType>))]
	[TfDboModelProperty("db_type")]
	public TfDatabaseColumnType DbType { get; set; }

	[TfDboModelProperty("include_table_search")]
	public bool IncludeInTableSearch { get; set; }
}
