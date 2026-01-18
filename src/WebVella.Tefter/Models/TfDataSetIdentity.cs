namespace WebVella.Tefter.Models;

public record TfDatasetIdentity
{
	public Guid Id { get; set; }

	public Guid DatasetId { get; set; }

	public string DataIdentity { get; set; }

	public List<string> Columns { get; set; } = new();
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