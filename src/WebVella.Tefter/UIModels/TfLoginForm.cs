using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfLoginForm
{
	[Required(ErrorMessage = "email required")]
	[EmailAddress(ErrorMessage = "invalid email")]
	public string Email { get; set; }

	[Required(ErrorMessage = "password required")]
	public string Password { get; set; }
	public bool RememberMe { get; set; } = true;
}
