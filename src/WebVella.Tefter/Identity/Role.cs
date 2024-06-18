namespace WebVella.Tefter.Identity;

[DboCacheModel]
[DboModel("role")]
public class Role
{
	[DboModelProperty("id")]
	public Guid Id { get; init; }

	[DboModelProperty("name")]
	public string Name { get; init; }
}
