namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitSpace(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Home
			|| routeState.FirstNode == RouteDataFirstNode.FastAccess
			|| routeState.FirstNode == RouteDataFirstNode.Space)
			)
		{
			result = result with { CurrentUserSpaces = new() };
			return result;
		}

		//CurrentUserSpaces
		if (result.CurrentUserSpaces.Count == 0) //Fill in only if not already loaded
			result = result with { CurrentUserSpaces = await GetUserSpacesAsync(currentUser) };


		return result;
	}

	internal Task<List<TucSpace>> GetUserSpacesAsync(TucUser user)
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
