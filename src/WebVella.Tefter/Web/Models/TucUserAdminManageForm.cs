﻿using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

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
	public TfColor ThemeColor { get; set; } = TfColor.Emerald500;
	[Required]
	public bool IsSidebarOpen { get; set; } = true;
	public TucCultureOption Culture { get; set; }

}
