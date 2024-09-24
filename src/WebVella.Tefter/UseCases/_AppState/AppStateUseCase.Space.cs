namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitSpaceAsync(TucUser currentUser, TfRouteState routeState, TfAppState newState, TfAppState oldState)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Home
			|| routeState.FirstNode == RouteDataFirstNode.Space)
			)
		{
			newState = newState with { CurrentUserSpaces = new(), Space = null };
			return newState;
		}

		//CurrentUserSpaces
		if (
			newState.CurrentUserSpaces.Count == 0
			|| (routeState.SpaceId is not null && !newState.CurrentUserSpaces.Any(x => x.Id == routeState.SpaceId))
			) //Fill in only if not already loaded
			newState = newState with { CurrentUserSpaces = await GetUserSpacesAsync(currentUser) };

		if (routeState.SpaceId is not null)
		{
			newState = newState with { Space = GetSpace(routeState.SpaceId.Value) };
		}
		else
		{
			newState = newState with { Space = null };
		}

		return newState;
	}

	internal TucSpace GetSpace(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpace(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return null;

		return new TucSpace(serviceResult.Value);
	}

	internal Task<List<TucSpace>> GetUserSpacesAsync(TucUser user)
	{

		var serviceResult = _spaceManager.GetSpacesListForUser(user.Id);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}
		var allSpaces = serviceResult.Value.Select(s => new TucSpace(s)).OrderBy(x => x.Position).ToList();
		var spacesHS = allSpaces.Select(x => x.Id).Distinct().ToHashSet();

		var viewSrvResult = _spaceManager.GetAllSpaceViews();
		if (viewSrvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetAllSpaceViews failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}

		var spaceViewsDict = new Dictionary<Guid, List<TfSpaceView>>();
		foreach (var item in viewSrvResult.Value)
		{
			if (!spacesHS.Contains(item.SpaceId)) continue;
			if (!spaceViewsDict.ContainsKey(item.SpaceId)) spaceViewsDict[item.SpaceId] = new();
			spaceViewsDict[item.SpaceId].Add(item);
		}

		foreach (var spaceId in spaceViewsDict.Keys)
		{
			spaceViewsDict[spaceId] = spaceViewsDict[spaceId].OrderBy(x => x.Position).ToList();
		}

		foreach (var space in allSpaces)
		{
			if (spaceViewsDict.ContainsKey(space.Id) && spaceViewsDict[space.Id].Count > 0)
				space.DefaultViewId = spaceViewsDict[space.Id].OrderBy(x=> x.Name).First().Id;
		}
		return Task.FromResult(allSpaces.OrderBy(x=> x.Position).Take(10).ToList());

	}

	internal Task<List<TucSpace>> GetAllSpaces()
	{

		var serviceResult = _spaceManager.GetSpacesList();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}
		var allSpaces = serviceResult.Value.Select(s => new TucSpace(s)).OrderBy(x => x.Position).ToList();
		var spacesHS = allSpaces.Select(x => x.Id).Distinct().ToHashSet();

		var viewSrvResult = _spaceManager.GetAllSpaceViews();
		if (viewSrvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetAllSpaceViews failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}

		var spaceViewsDict = new Dictionary<Guid, List<TfSpaceView>>();
		foreach (var item in viewSrvResult.Value)
		{
			if (!spacesHS.Contains(item.SpaceId)) continue;
			if (!spaceViewsDict.ContainsKey(item.SpaceId)) spaceViewsDict[item.SpaceId] = new();
			spaceViewsDict[item.SpaceId].Add(item);
		}

		foreach (var spaceId in spaceViewsDict.Keys)
		{
			spaceViewsDict[spaceId] = spaceViewsDict[spaceId].OrderBy(x => x.Position).ToList();
		}

		foreach (var space in allSpaces)
		{
			if (spaceViewsDict.ContainsKey(space.Id) && spaceViewsDict[space.Id].Count > 0)
				space.DefaultViewId = spaceViewsDict[space.Id].OrderBy(x=> x.Name).First().Id;
		}
		return Task.FromResult(allSpaces);

	}


	internal Result<TucSpace> CreateSpaceWithForm(TucSpace space)
	{
		var result = _spaceManager.CreateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceWithFormAsync failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}

	internal Result<TucSpace> UpdateSpaceWithForm(TucSpace space)
	{
		var result = _spaceManager.UpdateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("UpdateSpaceWithForm failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}
}
