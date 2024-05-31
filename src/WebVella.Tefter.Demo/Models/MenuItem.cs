using Microsoft.AspNetCore.Components.Routing;

namespace WebVella.Tefter.Demo.Models;

public class MenuItem
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public Icon Icon { get; set; }
	public string Url { get; set; }
	public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
}
