namespace WebVella.Tefter;

[DboCacheModel]
[DboModel("shared_column")]
public record TfSharedColumn
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("shared_key_db_name")]
	public string SharedKeyDbName { get; set; }

	[DboModelProperty("db_name")]
	public string DbName { get; set; }

	[DboTypeConverter(typeof(EnumPropertyConverter<DatabaseColumnType>))]
	[DboModelProperty("db_type")]
	public DatabaseColumnType DbType { get; set; }

	[DboModelProperty("include_table_search")]
	public bool IncludeInTableSearch { get; set; }

	[DboModelProperty("addon_id")]
	public Guid? AddonId { get; set; }
}
