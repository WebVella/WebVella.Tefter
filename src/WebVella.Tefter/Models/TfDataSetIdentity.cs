namespace WebVella.Tefter.Models;

public record TfDataSetIdentity
{
	public Guid Id { get; internal set; }

	public Guid DataSetId { get; internal set; }

	public string DataIdentity { get; set; }

	public bool IsSystem { get { return DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY;  } }

	public List<string> Columns { get; internal set; } = new();
}


[DboCacheModel]
[TfDboModel("tf_dataset_identity")]
internal record TfDataSetIdentityDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("dataset_id")]
	public Guid DataSetId { get; set; }

	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; }

	[TfDboModelProperty("column_names_json")]
	public string ColumnNamesJson { get; set; } = "[]";
}