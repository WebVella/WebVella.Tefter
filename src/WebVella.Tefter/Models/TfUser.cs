using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;

public class TfUser
{
	public Guid Id { get; init; }
	public string Email { get; init; } = null!;
	public string FirstName { get; init; } = null!;
	public string LastName { get; init; } = null!;
	internal string Password { get; init; } = null!;
	public bool Enabled { get; init; }
	public DateTime CreatedOn { get; init; }
	public TfUserSettings Settings { get; init; } = new();
	public ReadOnlyCollection<TfRole> Roles { get; init; } = null!;

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

	public bool IsAdmin
	{
		get
		{
			if (Roles is not null && Roles.Any(x => x.Id == TfConstants.ADMIN_ROLE_ID))
				return true;
			return false;
		}
	}
}


[DboCacheModel]
[TfDboModel("tf_user")]
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
	[TfDboTypeConverter(typeof(TfJsonPropertyConverter<TfUserSettings>))]
	public TfUserSettings Settings { get; set; }

	[TfDboModelProperty("x_search")]
	public string XSearch { get; set; }
}


[DboCacheModel]
[TfDboModel("tf_user_role")]
internal record UserRoleDbo
{
	[TfDboModelProperty("user_id")]
	public Guid UserId { get; set; }

	[TfDboModelProperty("role_id")]
	public Guid RoleId { get; set; }
}