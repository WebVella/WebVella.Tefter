namespace WebVella.Tefter.Identity;

[DboCacheModel]
[DboModel("user_role")]
internal record UserRoleRelation
{
	[DboModelProperty("user_id")]
	public Guid UserId { get; init; }

	[DboModelProperty("role_id")]
	public Guid RoleId { get; init; }
}
