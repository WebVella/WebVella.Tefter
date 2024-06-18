namespace WebVella.Tefter.Identity;

[DboCacheModel]
[DboModel("role")]
public class Role
{
	[DboModelProperty("id")]
	public Guid Id { get; init; }

	[DboModelProperty("name")]
	public string Name { get; init; }

	[DboModelProperty("is_system")]
	public bool IsSystem { get; init; } = false;
}
