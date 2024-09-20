﻿namespace WebVella.Tefter.Web.Models;

public class MenuItem
{
	public string Id { get; set; } //should not be just guid as there are problems
	public string Title { get; set; }
	public Icon Icon { get; set; }
	public OfficeColor IconColor { get; set; }
	public string OnClickUrl { get; set; }
	public string Url { get; set; }
	public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
	public bool Expanded { get; set; } = false;
	public bool Active { get; set; } = true;
	public int Level { get; set; } = 0;
	public string LevelClass { get => $"level-{Level}"; }
	public object Data { get; set; }
	public Guid? SpaceId { get; set; }
	public Guid? SpaceDataId { get; set; }
	public Guid? SpaceViewId { get; set; }

	public List<MenuItem> Nodes { get; set; } = new();

	[JsonIgnore]
	public Action<bool> OnSelect { get; set; }

	[JsonIgnore]
	public Action OnClick { get; set; }

	[JsonIgnore]
	public Action OnExpand { get; set; }

}
