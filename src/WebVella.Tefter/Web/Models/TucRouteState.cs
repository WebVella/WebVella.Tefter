namespace WebVella.Tefter.Web.Models;
public partial record TucRouteState
{
	public Dictionary<int, string> NodesDict { get; init; } = new();
	public Guid? SpaceId { get; init; }
	public Guid? SpaceDataId { get; init; }
	public Guid? SpaceViewId { get; init; }
	public Guid? SpacePageId { get; init; }
	public Guid? UserId { get; init; }
	public Guid? RoleId { get; init; }
	public Guid? DataProviderId { get; init; }
	public Guid? PageId { get; init; }
	public Guid? SpaceViewPresetId { get; init; }
	public string SpaceSection { get; init; }
	public TfTemplateResultType? TemplateResultType { get; init; }
	public Guid? TemplateId { get; init; }
	public List<RouteDataNode> RouteNodes { get; init; } = new List<RouteDataNode>();

	public int? Page { get; init; } = null;
	public int? PageSize { get; init; } = null;
	public string Search { get; init; } = null;
	public List<TucFilterBase> Filters { get; init; } = null;
	public List<TucSort> Sorts { get; init; } = null;
	public Guid? ActiveSaveId { get; init; } = null;
	public bool SearchInBookmarks { get; init; } = true;
	public bool SearchInSaves { get; init; } = true;
	public bool SearchInViews { get; init; } = true;

	public TucRouteState AddRouteNodes(params RouteDataNode[] nodes){ 
		var routeData = RouteNodes.ToList();
		routeData.AddRange(nodes);
		return this with { RouteNodes = routeData };
	}

	public bool HasNode(RouteDataNode node, int? index = null){ 
		if(RouteNodes is null) return false;
		if(index is null)
			return RouteNodes.Contains(node);

		if(RouteNodes.Count >= index + 1)
			return RouteNodes[index.Value] == node;

		return false;
	}
}

public enum RouteDataNode
{
	[Description("Home")]
	Home,
	[Description("Space")]
	Space,
	[Description("Admin")]
	Admin,
	[Description("Pages")]
	Pages,
	[Description("Users")]
	Users,
	[Description("Data Providers")]
	DataProviders,
	[Description("Shared Columns")]
	SharedColumns,
	[Description("Space View")]
	SpaceView,
	[Description("Space Dataset")]
	SpaceData,
	[Description("Space Page")]
	SpacePage,
	[Description("File Repository")]
	FileRepository,
	[Description("Templates")]
	Templates,
	[Description("Roles")]
	Roles,
	[Description("Details")]
	Details,
	[Description("Manage")]
	Manage,
	[Description("Access")]
	Access,
	[Description("Saves")]
	Saves,
	[Description("Schema")]
	Schema,
	[Description("Join Keys")]
	JoinKeys,
	[Description("Shared Columns")]
	Aux,
	[Description("Synchronization")]
	Synchronization,
	[Description("Data")]
	Data,
	[Description("Views")]
	Views,
	[Description("List")]
	List,
	[Description("User Id")]
	UserId,
	[Description("Role Id")]
	RoleId,
	[Description("Data Provider Id")]
	DataProviderId,
	[Description("Template Id")]
	TemplateId,
	[Description("Template Type Id")]
	TemplateTypeId,
	[Description("Space Id")]
	SpaceId,
	[Description("Space Page Id")]
	SpacePageId,
	[Description("Space Data Id")]
	SpaceDataId,
	[Description("Space View Id")]
	SpaceViewId,
	[Description("Page Id")]
	PageId,
}
