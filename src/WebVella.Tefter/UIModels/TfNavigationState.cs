namespace WebVella.Tefter.Models;
public partial record TfNavigationState
{
	public string Uri { get; init; } = default!;
	public Dictionary<int, string> NodesDict { get; init; } = new();
	public Guid? SpaceId { get; init; }
	public Guid? SpaceDataId { get; init; }
	public Guid? SpaceViewId { get; init; }
	public Guid? SpacePageId { get; init; }
	public Guid? UserId { get; init; }
	public Guid? RoleId { get; init; }
	public Guid? DataProviderId { get; init; }
	public string? DataIdentityId { get; init; }
	public Guid? SharedColumnId { get; init; }
	public Guid? PageId { get; init; }
	public Guid? SpaceViewPresetId { get; init; }
	public TfTemplateResultType? TemplateResultType { get; init; }
	public Guid? TemplateId { get; init; }
	public List<RouteDataNode> RouteNodes { get; init; } = new List<RouteDataNode>();

	public int? Page { get; init; } = null;
	public int? PageSize { get; init; } = null;
	public string? Search { get; init; } = null;
	public string? SearchAside { get; init; } = null;
	public List<TfFilterBase>? Filters { get; init; } = null;
	public List<TfSortQuery>? Sorts { get; init; } = null;
	public Guid? ActiveSaveId { get; init; } = null;
	public bool SearchInBookmarks { get; init; } = true;
	public bool SearchInSaves { get; init; } = true;
	public bool SearchInViews { get; init; } = true;
	public string? ReturnUrl { get; init; } = null;
	public TfNavigationState AddRouteNodes(params RouteDataNode[] nodes)
	{
		var routeData = RouteNodes.ToList();
		routeData.AddRange(nodes);
		return this with { RouteNodes = routeData };
	}

	public bool HasNode(RouteDataNode node, int? index = null)
	{
		if (RouteNodes is null) return false;
		if (index is null)
			return RouteNodes.Contains(node);

		if (RouteNodes.Count >= index + 1)
			return RouteNodes[index.Value] == node;

		return false;
	}
}