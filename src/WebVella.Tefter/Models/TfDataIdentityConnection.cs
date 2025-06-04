namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_data_identity_connection")]
public class TfDataIdentityConnection
{
	[TfDboModelProperty("source_data_identity")]
	public string SourceDataIdentity { get; set; }

	[TfDboModelProperty("source_data_value")]
	public string SourceDataValue { get; set; }

	[TfDboModelProperty("target_data_identity")]
	public string TargetDataIdentity { get; set; }

	[TfDboModelProperty("target_data_value")]
	public string TargetDataValue { get; set; }
}
