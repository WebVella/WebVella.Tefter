using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.UseCases.AppStart;

internal partial class AppStateUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly NavigationManager _navigationManager;

	public AppStateUseCase(
		AuthenticationStateProvider authenticationStateProvider,
		IJSRuntime jsRuntime,
		IIdentityManager identityManager,
		ITfSpaceManager spaceManager,
		NavigationManager navigationManager)
	{
		_authenticationStateProvider = authenticationStateProvider;
		_jsRuntime = jsRuntime;
		_identityManager = identityManager;
		_spaceManager = spaceManager;
		_navigationManager = navigationManager;
	}

	internal bool IsBusy { get; set; } = true;

	internal async Task<TfAppState> InitState(TucUser user, string url)
	{
		var result = new TfAppState();
		var routeState = _navigationManager.GetRouteState(url);

		if (routeState.FirstNode == RouteDataFirstNode.Home
		|| routeState.FirstNode == RouteDataFirstNode.FastAccess
		|| routeState.FirstNode == RouteDataFirstNode.Space)
		{
			result = result with { CurrentUserSpaces = await GetUserSpaces(user) };
		}
		else
		{
			result = result with { CurrentUserSpaces = new() };
		}

		return result;
	}

	internal Task<List<TucSpace>> GetUserSpaces(TucUser user)
	{

		var userSpaces = _spaceManager.GetSpacesListForUser(user.Id).Value.Select(s => new TucSpace(s)).OrderBy(x => x.Position).ToList();
		var spacesHS = userSpaces.Select(x => x.Id).Distinct().ToHashSet();
		var allViews = _spaceManager.GetAllSpaceViews().Value;
		var spaceViewsDict = new Dictionary<Guid, List<TfSpaceView>>();
		foreach (var item in allViews)
		{
			if (!spacesHS.Contains(item.SpaceId)) continue;
			if (!spaceViewsDict.ContainsKey(item.SpaceId)) spaceViewsDict[item.SpaceId] = new();
			spaceViewsDict[item.SpaceId].Add(item);
		}

		foreach (var spaceId in spaceViewsDict.Keys)
		{
			spaceViewsDict[spaceId] = spaceViewsDict[spaceId].OrderBy(x => x.Position).ToList();
		}

		foreach (var space in userSpaces)
		{
			if (spaceViewsDict.ContainsKey(space.Id) && spaceViewsDict[space.Id].Count > 0)
				space.DefaultViewId = spaceViewsDict[space.Id][0].Id;
		}
		return Task.FromResult(userSpaces);

	}


}
