namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitSpaceAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (newAppState.Route.FirstNode == RouteDataFirstNode.Admin)
		{
			newAppState = newAppState with { CurrentUserSpaces = new(), Space = null };
			return (newAppState, newAuxDataState);
		}

		//CurrentUserSpaces
		if (
			newAppState.CurrentUserSpaces.Count == 0
			|| (newAppState.Route.SpaceId is not null && !newAppState.CurrentUserSpaces.Any(x => x.Id == newAppState.Route.SpaceId))
			) //Fill in only if not already loaded
			newAppState = newAppState with { CurrentUserSpaces = await GetUserSpacesAsync(currentUser) };

		if (newAppState.Route.SpaceId is not null)
		{
			var space = GetSpace(newAppState.Route.SpaceId.Value);
			var spaceNodes = GetSpaceNodes(newAppState.Route.SpaceId.Value);
			var spacePageNode = spaceNodes.FindItemByMatch((x) => x.Type == TfSpaceNodeType.Page, (x) => x.ChildNodes);
			if (spacePageNode != null)
				space.DefaultNodeId = spacePageNode.Id;

			var spaceDataList = GetSpaceDataList(space.Id);
			var spaceViewList = GetSpaceViewList(space.Id);
			newAppState = newAppState with { Space = space, SpaceNodes = spaceNodes, SpaceDataList = spaceDataList, SpaceViewList = spaceViewList };
		}
		else
		{
			newAppState = newAppState with { Space = null };
		}

		return (newAppState, newAuxDataState);
	}

	internal virtual TucSpace GetSpace(
		Guid spaceId)
	{
		try
		{
			var space = _spaceManager.GetSpace(spaceId);
			if (space is null)
				return null;

			return new TucSpace(space);
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}

	internal virtual Task<List<TucSpace>> GetUserSpacesAsync(TucUser user)
	{
		try
		{
			var allSpaces = _spaceManager.GetSpacesListForUser(user.Id)
				.Select(s => new TucSpace(s))
				.OrderBy(x => x.Position)
				.ToList();

			var spacesHS = allSpaces.Select(x => x.Id).Distinct().ToHashSet();

			var allSpaceNodes = _spaceManager.GetAllSpaceNodes();

			var spaceNodeDict = new Dictionary<Guid, List<TfSpaceNode>>();
			foreach (var item in allSpaceNodes)
			{
				if (!spacesHS.Contains(item.SpaceId))
					continue;

				if (!spaceNodeDict.ContainsKey(item.SpaceId))
					spaceNodeDict[item.SpaceId] = new();

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
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}
	}

	internal virtual Task<List<TucSpace>> GetAllSpaces()
	{
		try
		{
			var allSpaces = _spaceManager.GetSpacesList()
				.Select(s => new TucSpace(s))
				.OrderBy(x => x.Position)
				.ToList();

			var spacesHS = allSpaces
				.Select(x => x.Id)
				.Distinct()
				.ToHashSet();

			var spaceViewsDict = new Dictionary<Guid, List<TfSpaceView>>();
			foreach (var item in _spaceManager.GetAllSpaceViews())
			{
				if (!spacesHS.Contains(item.SpaceId))
					continue;

				if (!spaceViewsDict.ContainsKey(item.SpaceId))
					spaceViewsDict[item.SpaceId] = new();

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
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpace>());
		}
	}

	internal virtual TucSpace CreateSpaceWithForm(
		TucSpace space)
	{
		var result = _spaceManager.CreateSpace(space.ToModel());
		return new TucSpace(result);
	}

	internal virtual TucSpace UpdateSpaceWithForm(
		TucSpace space)
	{
		var result = _spaceManager.UpdateSpace(space.ToModel());
		return new TucSpace(result);
	}

	internal virtual void DeleteSpace(
		Guid spaceId)
	{
		_spaceManager.DeleteSpace(spaceId);
	}

	internal virtual List<TucSpaceNode> GetSpaceNodes(
		Guid spaceId)
	{
		try
		{
			return _spaceManager.GetSpaceNodes(spaceId)
				.Select(x => new TucSpaceNode(x))
				.ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}

	internal virtual List<TucSpaceNode> CreateSpaceNode(
		TucSpaceNode node)
	{
		return _spaceManager.CreateSpaceNode(node.ToModel())
				.Item2
				.Select(x => new TucSpaceNode(x)).ToList();
	}

	internal virtual List<TucSpaceNode> UpdateSpaceNode(
		TucSpaceNode node)
	{
		return _spaceManager.UpdateSpaceNode(node.ToModel())
			.Select(x => new TucSpaceNode(x))
			.ToList();
	}

	internal virtual List<TucSpaceNode> MoveSpaceNode(
		TucSpaceNode node,
		bool isMoveUp)
	{
		if (isMoveUp) 
			node.Position--;
		else 
			node.Position++;

		return UpdateSpaceNode(node);
	}

	internal virtual List<TucSpaceNode> DeleteSpaceNode(
		TucSpaceNode node)
	{
		return _spaceManager.DeleteSpaceNode(node.ToModel())
			.Select(x => new TucSpaceNode(x))
			.ToList();
	}

	internal virtual List<TucSpaceNode> CopySpaceNode(Guid nodeId)
	{
		//newNodeId is the id of newly created node copy of specified one by nodeId argument
		var (newNodeId, nodesList) = _spaceManager.CopySpaceNode(nodeId);
		return nodesList.Select(x => new TucSpaceNode(x)).ToList();
	}
}
