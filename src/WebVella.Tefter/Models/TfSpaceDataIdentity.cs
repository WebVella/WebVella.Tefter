namespace WebVella.Tefter.Models;

public record TfSpaceDataIdentity
{
	public Guid Id { get; internal set; }

	public Guid SpaceDataId { get; internal set; }

	public string DataIdentity { get; set; }

	public bool IsSystem { get { return DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY;  } }

	public List<string> Columns { get; internal set; } = new();
}


[DboCacheModel]
[TfDboModel("tf_space_data_identity")]
internal record TfSpaceDataIdentityDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("space_data_id")]
	public Guid SpaceDataId { get; set; }

	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; }

	[TfDboModelProperty("column_names_json")]
	public string ColumnNamesJson { get; set; } = "[]";
}