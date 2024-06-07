using Bogus;

namespace WebVella.Tefter.Demo.Models;

public class UserPermissions
{
	public Permission Id { get; set; }

}


public class Permission
{
	public bool Can { get; set; } = true;
	public string Reason { get; set; } = null; // null if true, string to show the user if yes

}