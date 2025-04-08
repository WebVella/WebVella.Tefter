namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<(TfAppState, TfAuxDataState)> InitPagesAsync(IServiceProvider serviceProvider,
		TucUser currentUser, 
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (newAppState.Route.FirstNode == RouteDataFirstNode.Admin)
		{
			newAppState = newAppState with
			{
				Pages = GetRegionComponentsMetaForContext(
					context: typeof(TfAdminPageScreenRegion),
					scope: null
				)
			};
		}
		else
		{
			newAppState = newAppState with
			{
				Pages = GetRegionComponentsMetaForContext(
					context: typeof(TfPageScreenRegion),
					scope: null
				)
			};
		}

		foreach (TfRegionComponentMeta addonComponent in newAppState.Pages)
		{
			if (addonComponent.ComponentType.ImplementsInterface(typeof(ITucAuxDataUseComponent)))
			{
				var component = (ITucAuxDataUseComponent)Activator.CreateInstance(addonComponent.ComponentType);
				component.OnAppStateInit(
						serviceProvider: _serviceProvider,
						currentUser: currentUser,
						newAppState: newAppState,
						oldAppState: oldAppState,
						newAuxDataState: newAuxDataState,
						oldAuxDataState: oldAuxDataState
				);
			}
		}

		return Task.FromResult((newAppState,newAuxDataState));
	}

	internal virtual ReadOnlyCollection<TfSpaceNodeComponentMeta> GetSpaceNodeComponents()
	{
		return _metaService.GetSpaceNodesComponentsMeta();
	}
	
}
