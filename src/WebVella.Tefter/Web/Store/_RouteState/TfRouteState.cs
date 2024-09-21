﻿namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;

[FeatureState]
public partial record TfRouteState
{
	public Dictionary<int, string> NodesDict { get; init; } = new();
	public Guid? SpaceId { get; init; }
	public Guid? SpaceDataId { get; init; }
	public Guid? SpaceViewId { get; init; }
	public Guid? UserId { get; init; }
	public Guid? DataProviderId { get; init; }
	public string SpaceSection { get; init; }
	public RouteDataFirstNode FirstNode { get; init; } = RouteDataFirstNode.Home;
	public RouteDataSecondNode SecondNode { get; init; } = default!;
	public RouteDataThirdNode ThirdNode { get; init; } = default!;

	public int? Page { get; init;} = 1;
	public int? PageSize { get; init;} = TfConstants.PageSize;
	public string Search { get; init;} = null;
	public List<TucFilterBase> Filters { get; init;} = null;
	public List<TucSort> Sorts { get; init;} = null;
	public Guid? ActiveSaveId { get; init;} = null;
	public bool SearchInBookmarks { get; init;} = true;
	public bool SearchInSaves { get; init;} = true;
	public bool SearchInViews { get; init;} = true;
	public bool SearchInSpaces { get; init;} = true;
}

public enum RouteDataFirstNode
{
	Home = 0,
	Space = 1,
	Admin = 2
}

public enum RouteDataSecondNode
{
	Dashboard = 0,
	Users = 1,
	DataProviders = 2,
	SharedColumns = 3,
	SpaceView = 4,
	SpaceData = 5
}

public enum RouteDataThirdNode
{
	Details = 0,
	Manage = 1,
	Access = 2,
	Saves = 3,
	Schema = 4,
	SharedKeys = 5,
	AuxColumns = 6,
	Synchronization = 7,
	Data = 8,
	Views = 9

}