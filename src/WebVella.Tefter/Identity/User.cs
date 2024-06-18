using System.Net.Mail;

namespace WebVella.Tefter.Identity;

[DboCacheModel]
[DboModel("user")]
public record User
{
	[DboModelProperty("id")]
	public Guid Id { get; init; }

	[DboModelProperty("email")]
	public string Email { get; init; }

	[DboModelProperty("password")]
	internal string Password { get; init; }

	[DboModelProperty("first_name")]
	public string FirstName { get; init; }

	[DboModelProperty("last_name")]
	public string LastName { get; init; }

	[DboModelProperty("enabled")]
	public bool Enabled { get; init; }

	[DboModelProperty("created_on")]
	[DboTypeConverter(typeof(LegacyDateTimePropertyConverter))]
	public DateTime CreatedOn { get; set; }

	[DboModelProperty("settings_json")]
	[DboTypeConverter(typeof(JsonPropertyConverter<UserSettings>))]
	public UserSettings Settings { get; init; }


	//non database properties
	public List<Role> Roles { get; set; } = new List<Role>();
}
