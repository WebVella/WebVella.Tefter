namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitSpaceNodeAsync(IServiceProvider serviceProvider,
		TucUser currentUser, 
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
			return (newAppState,newAuxDataState);
		}
		TucSpaceNode spaceNode = null;
		if (newAppState.Route.SpacePageId is not null)
		{
			spaceNode = newAppState.SpaceNodes.FindItemByMatch(
				matcher:(x)=> x.Id == newAppState.Route.SpacePageId.Value,
				childGetter:(x)=> x.ChildNodes
			);
			newAppState = newAppState with { SpaceNode = spaceNode };
			if (spaceNode is not null)
			{
				var nodeComponentsMeta = GetSpaceNodeComponents();
				var spaceNodeMeta = nodeComponentsMeta.FirstOrDefault(x => x.ComponentId == spaceNode.ComponentId);
				if (spaceNodeMeta is not null)
				{
					var context = new TfSpacePageAddonContext
					{
						ComponentOptionsJson = spaceNode.ComponentOptionsJson,
						Icon = spaceNode.Icon,
						Mode = TfComponentMode.Read,
						SpaceId = spaceNode.SpaceId,
						SpacePageId = spaceNode.Id
					};
					var(appStateResult, auxDataStateResult) = await spaceNodeMeta.Instance.InitState(
					serviceProvider:serviceProvider,
					currentUser: currentUser, 
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



		return (newAppState,newAuxDataState);
	}

}
