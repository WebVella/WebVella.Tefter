namespace WebVella.Tefter.Models;

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
[TfDboModel("data_provider_shared_key")]
internal record TfDataProviderSharedKeyDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[TfDboModelProperty("db_name")]
	public string DbName { get; set; }

	[TfDboModelProperty("description")]
	public string Description { get; set; }

	[TfDboModelProperty("column_ids_json")]
	public string ColumnIdsJson { get; set; } = "[]";

	[TfDboModelProperty("version")]
	public short Version { get; set; }

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("last_modified_on")]
	public DateTime LastModifiedOn { get; set; }
}