using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucUserAdminManageForm
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string FirstName { get; set; }
	[Required]
	public string LastName { get; set; }
	internal string Password { get; set; }
	internal string ConfirmPassword { get; set; }
	[Required]
	public bool Enabled { get; set; } = true;
	[Required]
	public DesignThemeModes ThemeMode { get; set; } = DesignThemeModes.System;
	[Required]
	public OfficeColor ThemeColor { get; set; } = OfficeColor.Excel;
	[Required]
	public bool IsSidebarOpen { get; set; } = true;
	public TucCultureOption Culture { get; set; }
	public List<TucRole> Roles { get; set; } = new();

	public void OnRoleChange(TucRole role)
	{
		if (Roles.Any(x => x.Id == role.Id))
		{
			Roles = Roles.Where(x => x.Id != role.Id).ToList();
		}
		else
		{
			Roles.Add(role);
		}
	}

}
