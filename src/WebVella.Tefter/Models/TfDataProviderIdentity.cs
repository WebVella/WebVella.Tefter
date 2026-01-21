namespace WebVella.Tefter.Models;

public record TfDataProviderIdentity
{
	public Guid Id { get; set; }

	public Guid DataProviderId { get; set; }

	public string DataIdentity { get; set; }

	public string Prefix { get; set; } = string.Empty;

	public List<string> Columns { get; set; } = new();
}


[DboCacheModel]
[TfDboModel("tf_data_provider_identity")]
internal record TfDataProviderIdentityDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; }

	[TfDboModelProperty("column_names_json")]
	public string ColumnNamesJson { get; set; } = "[]";

	[TfDboModelProperty("prefix")]
	public string Prefix { get; set; } = string.Empty;
}