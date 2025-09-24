namespace WebVella.Tefter.Models;

public record TfDatasetIdentity
{
	public Guid Id { get; internal set; }

	public Guid DatasetId { get; internal set; }

	public string DataIdentity { get; set; }

	public bool IsSystem { get { return DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY;  } }

	public List<string> Columns { get; internal set; } = new();
}


[DboCacheModel]
[TfDboModel("tf_dataset_identity")]
internal record TfDatasetIdentityDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("dataset_id")]
	public Guid DatasetId { get; set; }

	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; }

	[TfDboModelProperty("column_names_json")]
	public string ColumnNamesJson { get; set; } = "[]";
}