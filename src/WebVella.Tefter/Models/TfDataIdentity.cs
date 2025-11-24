namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_data_identity")]
public class TfDataIdentity
{
	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; } = null!;

	[TfDboModelProperty("label")] public string? Label { get; set; } = null;

	public bool IsSystem { get { return DataIdentity == TfConstants.TEFTER_DEFAULT_OBJECT_NAME; } }

	public override string ToString()
	{
		return DataIdentity;
	}
}
