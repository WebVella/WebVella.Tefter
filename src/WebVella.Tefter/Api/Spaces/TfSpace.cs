namespace WebVella.Tefter;
public class TfSpace
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int Position { get; set; }
	public bool IsPrivate { get; set; }
	public string Icon { get; set; }
	public short Color { get; set; }
}

[DboCacheModel]
[DboModel("space")]
internal class TfSpaceDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboModelProperty("position")]
	[DboTypeConverter(typeof(IntegerPropertyConverter))]
	public int Position { get; set; }

	[DboModelProperty("is_private")]
	public bool IsPrivate { get; set; }

	[DboModelProperty("icon")]
	public string Icon { get; set; }

	[DboModelProperty("color")]
	public short Color { get; set; }
}
