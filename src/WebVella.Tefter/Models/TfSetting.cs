namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_setting")]
public class TfSetting
{
	[TfDboModelProperty("name")]
	public string Key { get; set; }

	[TfDboModelProperty("value")]
	public string Value { get; set; }
}
