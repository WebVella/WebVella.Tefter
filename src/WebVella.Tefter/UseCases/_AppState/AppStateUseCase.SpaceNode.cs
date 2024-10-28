namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitSpaceNodeAsync(IServiceProvider serviceProvider,
		TucUser currentUser, TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (newAppState.Space is null)
		{
			newAppState = newAppState with
			{
				SpaceNode = null,
				SpaceNodeData = null
			};
			return newAppState;
		}

		TucSpaceNode spaceNode = null;
		if (routeState.SpaceNodeId is not null)
		{
			spaceNode = newAppState.SpaceNodes.FindItemByMatch(
				matcher:(x)=> x.Id == routeState.SpaceNodeId.Value,
				childGetter:(x)=> x.ChildNodes
			);
			newAppState = newAppState with { SpaceNode = spaceNode };
			if (spaceNode is not null)
			{
				var nodeComponentsMeta = GetSpaceNodeComponents();
				var spaceNodeMeta = nodeComponentsMeta.FirstOrDefault(x => x.ComponentType.FullName == spaceNode.ComponentTypeFullName);
				if (spaceNodeMeta is not null)
				{
					var context = new TfSpaceNodeComponentContext
					{
						ComponentOptionsJson = spaceNode.ComponentOptionsJson,
						Icon = spaceNode.Icon,
						Mode = TfComponentMode.Read,
						SpaceId = spaceNode.SpaceId,
						SpaceNodeId = spaceNode.Id
					};
					var(appStateResult, auxDataStateResult) = await spaceNodeMeta.Instance.InitState(
					serviceProvider:serviceProvider,
					currentUser: currentUser, 
					routeState: routeState, 
					newAppState: newAppState,
					oldAppState: oldAppState,
					newAuxDataState: newAuxDataState,
					oldAuxDataState: oldAuxDataState, 
					context: context);

					newAppState = appStateResult;
					newAuxDataState = auxDataStateResult;
				}
			}
		}



		return newAppState;
	}

}
