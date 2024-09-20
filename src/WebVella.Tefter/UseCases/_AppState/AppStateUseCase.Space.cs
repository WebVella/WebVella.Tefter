namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitSpaceAsync(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Home
			|| routeState.FirstNode == RouteDataFirstNode.FastAccess
			|| routeState.FirstNode == RouteDataFirstNode.Space)
			)
		{
			result = result with { CurrentUserSpaces = new(), Space = null };
			return result;
		}

		//CurrentUserSpaces
		if (
			result.CurrentUserSpaces.Count == 0
			|| (routeState.SpaceId is not null && !result.CurrentUserSpaces.Any(x => x.Id == routeState.SpaceId))
			) //Fill in only if not already loaded
			result = result with { CurrentUserSpaces = await GetUserSpacesAsync(currentUser) };

		if (routeState.SpaceId is not null)
		{
			result = result with { Space = GetSpace(routeState.SpaceId.Value) };
		}
		else
		{
			result = result with { Space = null };
		}

		return result;
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
				space.DefaultViewId = spaceViewsDict[space.Id][0].Id;
		}
		return Task.FromResult(allSpaces);

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
				space.DefaultViewId = spaceViewsDict[space.Id][0].Id;
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
