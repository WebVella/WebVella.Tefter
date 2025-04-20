namespace WebVella.Tefter.Web.Models;
public partial record TucRouteState
{
	public Dictionary<int, string> NodesDict { get; init; } = new();
	public Guid? SpaceId { get; init; }
	public Guid? SpaceDataId { get; init; }
	public Guid? SpaceViewId { get; init; }
	public Guid? SpaceNodeId { get; init; }
	public Guid? UserId { get; init; }
	public Guid? DataProviderId { get; init; }
	public Guid? SpaceViewPresetId { get; init; }
	public string SpaceSection { get; init; }
	public TfTemplateResultType? TemplateResultType { get; init; }
	public Guid? TemplateId { get; init; }
	public RouteDataFirstNode FirstNode { get; init; } = RouteDataFirstNode.Home;
	public RouteDataSecondNode SecondNode { get; init; } = default!;
	public RouteDataThirdNode ThirdNode { get; init; } = default!;


	public int? Page { get; init; } = null;
	public int? PageSize { get; init; } = null;
	public string Search { get; init; } = null;
	public List<TucFilterBase> Filters { get; init; } = null;
	public List<TucSort> Sorts { get; init; } = null;
	public Guid? ActiveSaveId { get; init; } = null;
	public bool SearchInBookmarks { get; init; } = true;
	public bool SearchInSaves { get; init; } = true;
	public bool SearchInViews { get; init; } = true;
}

public enum RouteDataFirstNode
{
	[Description("Home")]
	Home = 0,
	[Description("Space")]
	Space = 1,
	[Description("Administration")]
	Admin = 2,
	[Description("Pages")]
	Pages = 3,
}

public enum RouteDataSecondNode
{
	[Description("Dashboard")]
	Dashboard = 0,
	[Description("Users")]
	Users = 1,
	[Description("Data Providers")]
	DataProviders = 2,
	[Description("Shared Columns")]
	SharedColumns = 3,
	[Description("Space View")]
	SpaceView = 4,
	[Description("Space Dataset")]
	SpaceData = 5,
	[Description("Pages")]
	Pages = 6,
	[Description("Space Page")]
	SpacePage = 7,
	[Description("File Repository")]
	FileRepository = 8,
	[Description("Templates")]
	Templates = 9,
}

public enum RouteDataThirdNode
{
	[Description("Details")]
	Details = 0,
	[Description("Manage")]
	Manage = 1,
	[Description("Access")]
	Access = 2,
	[Description("Saves")]
	Saves = 3,
	[Description("Schema")]
	Schema = 4,
	[Description("Join Keys")]
	JoinKeys = 5,
	[Description("Shared Columns")]
	AuxColumns = 6,
	[Description("Synchronization")]
	Synchronization = 7,
	[Description("Data")]
	Data = 8,
	[Description("Views")]
	Views = 9,
	[Description("List")]
	List = 10

}