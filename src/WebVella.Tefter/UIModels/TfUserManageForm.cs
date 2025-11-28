using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfUserManageForm
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	[EmailAddress]
	public string Email { get; set; } = null!;
	[Required]
	public string FirstName { get; set; } = null!;
	[Required]
	public string LastName { get; set; } = null!;
	internal string? Password { get; set; }
	internal string? ConfirmPassword { get; set; }
	[Required]
	public bool Enabled { get; set; } = true;
	
	[Required]
	public TfColor Color { get; set; } = TfConstants.DefaultThemeColor;	
	
	[Required]
	public DesignThemeModes ThemeMode { get; set; } = DesignThemeModes.System;
	[Required]
	public bool IsSidebarOpen { get; set; } = true;
	public TfCultureOption? Culture { get; set; } = null;

}

