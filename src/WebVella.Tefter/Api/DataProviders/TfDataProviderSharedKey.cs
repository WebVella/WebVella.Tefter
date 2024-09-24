namespace WebVella.Tefter;

public record TfDataProviderSharedKey
{
	public Guid Id { get; internal set; }

	public Guid DataProviderId { get; internal set; }

	public string DbName { get; internal set; }

	public string Description { get; internal set; }

	public List<TfDataProviderColumn> Columns { get; internal set; } = new();

	public short Version { get; internal set; }

	public DateTime LastModifiedOn { get; internal set; }
}


[DboCacheModel]
[DboModel("data_provider_shared_key")]
internal record TfDataProviderSharedKeyDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[DboModelProperty("db_name")]
	public string DbName { get; set; }

	[DboModelProperty("description")]
	public string Description { get; set; }

	[DboModelProperty("column_ids_json")]
	public string ColumnIdsJson { get; set; } = "[]";

	[DboModelProperty("version")]
	public short Version { get; set; }

	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	[DboModelProperty("last_modified_on")]
	public DateTime LastModifiedOn { get; set; }
}