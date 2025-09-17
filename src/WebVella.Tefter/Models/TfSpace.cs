namespace WebVella.Tefter.Models;
public record TfSpace
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public short Position { get; set; }
	public bool IsPrivate { get; set; }
	public string FluentIconName { get; set; }
	public TfColor? Color { get; set; }
	public List<TfRole> Roles { get; set; } = new();
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

[DboCacheModel]
[TfDboModel("tf_space_role")]
internal record SpaceRoleDbo
{
	[TfDboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[TfDboModelProperty("role_id")]
	public Guid RoleId { get; set; }
}