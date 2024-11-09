

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

	public string Names
	{
		get
		{
			var sb = new List<string>();
			if (!String.IsNullOrWhiteSpace(FirstName)) sb.Add(FirstName);
			if (!String.IsNullOrWhiteSpace(LastName)) sb.Add(LastName);

			return String.Join(" ", sb);
		}
	}
}

public class Role
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	internal bool IsSystem { get; init; }
}

public class UserSettings
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
	public bool IsSidebarOpen { get; init; } = true;
	public string CultureName { get; init; } = string.Empty;
	public string StartUpUrl { get; init; } = null;
	public int? PageSize { get; init; } = null;
}



[DboCacheModel]
[TfDboModel("user")]
internal record UserDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("email")]
	public string Email { get; set; }

	[TfDboModelProperty("password")]
	public string Password { get; set; }

	[TfDboModelProperty("first_name")]
	public string FirstName { get; set; }

	[TfDboModelProperty("last_name")]
	public string LastName { get; set; }

	[TfDboModelProperty("enabled")]
	public bool Enabled { get; set; }

	[TfDboModelProperty("created_on")]
	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	public DateTime CreatedOn { get; set; }

	[TfDboModelProperty("settings_json")]
	[TfDboTypeConverter(typeof(TfJsonPropertyConverter<UserSettings>))]
	public UserSettings Settings { get; set; }

	[TfDboModelProperty("x_search")]
	public string XSearch { get; set; }
}


[DboCacheModel]
[TfDboModel("user_role")]
internal record UserRoleDbo
{
	[TfDboModelProperty("user_id")]
	public Guid UserId { get; set; }

	[TfDboModelProperty("role_id")]
	public Guid RoleId { get; set; }
}


[DboCacheModel]
[TfDboModel("role")]
public record RoleDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("is_system")]
	public bool IsSystem { get; set; } = false;
}

