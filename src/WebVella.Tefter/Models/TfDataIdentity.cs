namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_data_identity")]
public class TfDataIdentity
{
	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; } = null!;

	[TfDboModelProperty("label")] public string? Label { get; set; } = null;

	[Obsolete("There are no system data identities anymore. To be removed.")]
	public bool IsSystem { get { return false; } }

	public override string ToString()
	{
		return DataIdentity;
	}
}
