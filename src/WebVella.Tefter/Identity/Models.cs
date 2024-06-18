namespace WebVella.Tefter.Identity;

public class User
{
	public Guid Id { get; init; }
	public string Email { get; init; }
	public string FirstName { get; init; }
	public string LastName { get; init; }
	internal string Password { get; init; }
	public bool Enabled { get; init; }
	public DateTime CreatedOn { get; init; }
	public UserSettings Settings { get; init; } = new();
	public ReadOnlyCollection<Role> Roles { get; init; }
}

public class Role
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	internal bool IsSystem { get; init; }
}

public class UserSettings
{
	public string UiTheme { get; init; }

	public string UiColor { get; init; }

	public bool IsSidebarOpen { get; init; }
}



[DboCacheModel]
[DboModel("user")]
internal record UserDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("email")]
	public string Email { get; set; }

	[DboModelProperty("password")]
	public string Password { get; set; }

	[DboModelProperty("first_name")]
	public string FirstName { get; set; }

	[DboModelProperty("last_name")]
	public string LastName { get; set; }

	[DboModelProperty("enabled")]
	public bool Enabled { get; set; }

	[DboModelProperty("created_on")]
	[DboTypeConverter(typeof(LegacyDateTimePropertyConverter))]
	public DateTime CreatedOn { get; set; }

	[DboModelProperty("settings_json")]
	[DboTypeConverter(typeof(JsonPropertyConverter<UserSettings>))]
	public UserSettings Settings { get; set; }

	[DboModelProperty("x_search")]
	public string XSearch { get; set; }
}


[DboCacheModel]
[DboModel("user_role")]
internal record UserRoleDbo
{
	[DboModelProperty("user_id")]
	public Guid UserId { get; set; }

	[DboModelProperty("role_id")]
	public Guid RoleId { get; set; }
}


[DboCacheModel]
[DboModel("role")]
public record RoleDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboModelProperty("is_system")]
	public bool IsSystem { get; set; } = false;
}

