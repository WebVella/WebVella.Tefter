namespace WebVella.Tefter.Models;

public class TfRole
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	internal bool IsSystem { get; set; }
}

[DboCacheModel]
[TfDboModel("tf_role")]
public record RoleDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("is_system")]
	public bool IsSystem { get; set; } = false;
}
