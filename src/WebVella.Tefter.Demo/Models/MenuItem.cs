using Microsoft.AspNetCore.Components.Routing;

namespace WebVella.Tefter.Demo.Models;

public class MenuItem
{
	public string Id { get; set; } //should not be just guid as there are problems
	public string Title { get; set; }
	public Icon Icon { get; set; }
	public string Url { get; set; }
	public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
}
