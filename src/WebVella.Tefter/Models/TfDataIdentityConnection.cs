namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_data_identity_connection")]
public class TfDataIdentityConnection
{
	[JsonPropertyName("data_identity_1")]
	[TfDboModelProperty("data_identity_1")]
	public string DataIdentity1 { get; set; }

	[JsonPropertyName("value_1")]
	[TfDboModelProperty("value_1")]
	public string Value1 { get; set; }

	[JsonPropertyName("data_identity_2")]
	[TfDboModelProperty("data_identity_2")]
	public string DataIdentity2 { get; set; }

	[JsonPropertyName("value_2")]
	[TfDboModelProperty("value_2")]
	public string Value2 { get; set; }
}
