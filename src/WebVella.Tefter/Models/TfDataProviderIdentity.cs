namespace WebVella.Tefter.Models;

public record TfDataProviderIdentity
{
	public Guid Id { get; internal set; }

	public Guid DataProviderId { get; internal set; }

	public string DataIdentity { get; set; }

	public bool IsSystem { get { return DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY;  } }

	public List<string> Columns { get; internal set; } = new();
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
}