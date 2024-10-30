namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitSpaceAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser, TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (routeState.FirstNode == RouteDataFirstNode.Admin)
		{
			newAppState = newAppState with { CurrentUserSpaces = new(), Space = null };
			return (newAppState,newAuxDataState);
		}

		//CurrentUserSpaces
		if (
			newAppState.CurrentUserSpaces.Count == 0
			|| (routeState.SpaceId is not null && !newAppState.CurrentUserSpaces.Any(x => x.Id == routeState.SpaceId))
			) //Fill in only if not already loaded
			newAppState = newAppState with { CurrentUserSpaces = await GetUserSpacesAsync(currentUser) };

		if (routeState.SpaceId is not null)
		{
			var space = GetSpace(routeState.SpaceId.Value);
			var spaceNodes = GetSpaceNodes(routeState.SpaceId.Value);
			
			newAppState = newAppState with { Space = space, SpaceNodes = spaceNodes };
		}
		else
		{
			newAppState = newAppState with { Space = null };
		}

		return (newAppState,newAuxDataState);
	}

	internal TucSpace GetSpace(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpace(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
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
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}
		var allSpaces = serviceResult.Value.Select(s => new TucSpace(s)).OrderBy(x => x.Position).ToList();
		var spacesHS = allSpaces.Select(x => x.Id).Distinct().ToHashSet();

		var nodeSrvResult = _spaceManager.GetAllSpaceNodes();
		if (nodeSrvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetAllSpaceNodes failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}

		var spaceNodeDict = new Dictionary<Guid, List<TfSpaceNode>>();
		foreach (var item in nodeSrvResult.Value)
		{
			if (!spacesHS.Contains(item.SpaceId)) continue;
			if (!spaceNodeDict.ContainsKey(item.SpaceId)) spaceNodeDict[item.SpaceId] = new();
			spaceNodeDict[item.SpaceId].Add(item);
		}

		foreach (var space in allSpaces)
		{
			if (spaceNodeDict.ContainsKey(space.Id) && spaceNodeDict[space.Id].Count > 0)
			{
				var spacePageNode = spaceNodeDict[space.Id].FindItemByMatch((x) => x.Type == TfSpaceNodeType.Page, (x) => x.ChildNodes);
				if (spacePageNode != null)
					space.DefaultNodeId = spacePageNode.Id;
			}

		}
		return Task.FromResult(allSpaces.OrderBy(x => x.Position).Take(10).ToList());

	}
	internal Task<List<TucSpace>> GetAllSpaces()
	{

		var serviceResult = _spaceManager.GetSpacesList();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
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
				toastValidationMessage: "Invalid Data",
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
				space.DefaultNodeId = spaceViewsDict[space.Id].OrderBy(x => x.Name).First().Id;
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

	internal Result DeleteSpace(Guid spaceId)
	{
		var result = _spaceManager.DeleteSpace(spaceId);
		if (result.IsFailed) return Result.Fail(new Error("DeleteSpace failed").CausedBy(result.Errors));
		return Result.Ok();
	}

	internal List<TucSpaceNode> GetSpaceNodes(Guid spaceId)
	{
		var resultSM = _spaceManager.GetSpaceNodes(spaceId);
		if (resultSM.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(resultSM.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		var result = resultSM.Value.Select(x => new TucSpaceNode(x)).ToList();
		return result;
	}

	internal Result<List<TucSpaceNode>> CreateSpaceNode(TucSpaceNode node)
	{
		var resultSM = _spaceManager.CreateSpaceNode(node.ToModel());
		if (resultSM.IsFailed) return Result.Fail(new Error("CreateSpaceNode failed").CausedBy(resultSM.Errors));
		var result = resultSM.Value.Item2.Select(x => new TucSpaceNode(x)).ToList();
		return result;
	}

	internal Result<List<TucSpaceNode>> UpdateSpaceNode(TucSpaceNode node)
	{
		var resultSM = _spaceManager.UpdateSpaceNode(node.ToModel());
		if (resultSM.IsFailed) return Result.Fail(new Error("UpdateSpaceNode failed").CausedBy(resultSM.Errors));
		var result = resultSM.Value.Select(x => new TucSpaceNode(x)).ToList();
		return result;
	}

	internal Result<List<TucSpaceNode>> MoveSpaceNode(TucSpaceNode node, bool isMoveUp)
	{
		if (isMoveUp) node.Position--;
		else node.Position++;
		return UpdateSpaceNode(node);
	}

	internal Result<List<TucSpaceNode>> DeleteSpaceNode(TucSpaceNode node)
	{
		var resultSM = _spaceManager.DeleteSpaceNode(node.ToModel());
		if (resultSM.IsFailed) return Result.Fail(new Error("DeleteSpaceNode failed").CausedBy(resultSM.Errors));
		var result = resultSM.Value.Select(x => new TucSpaceNode(x)).ToList();
		return result;
	}

	internal Result<List<TucSpaceNode>> CopySpaceNode(Guid nodeId)
	{
		var resultSM = _spaceManager.CopySpaceNode(nodeId);
		if (resultSM.IsFailed) return Result.Fail(new Error("CopySpaceNode failed").CausedBy(resultSM.Errors));
		//newNodeId is the id of newly created node copy of specified one by nodeId argument
		var (newNodeId, nodesList) = resultSM.Value;
		var result = nodesList.Select(x => new TucSpaceNode(x)).ToList();
		return result;
	}
}
