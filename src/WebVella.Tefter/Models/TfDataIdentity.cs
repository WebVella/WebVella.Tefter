namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_data_identity")]
public class TfDataIdentity
{
	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; }

	[TfDboModelProperty("label")]
	public string Label { get; set; }

	public bool IsSystem { get { return DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY; } }
}
