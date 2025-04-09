namespace WebVella.Tefter.Models;
public class TfSpace
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public short Position { get; set; }
	public bool IsPrivate { get; set; }
	public string Icon { get; set; }
	public short Color { get; set; }
}

[DboCacheModel]
[TfDboModel("tf_space")]
internal class TfSpaceDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("is_private")]
	public bool IsPrivate { get; set; }

	[TfDboModelProperty("icon")]
	public string Icon { get; set; }

	[TfDboModelProperty("color")]
	public short Color { get; set; }
}
