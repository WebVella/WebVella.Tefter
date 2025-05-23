﻿namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<(TfAppState, TfAuxDataState)> InitPagesAsync(IServiceProvider serviceProvider,
		TucUser currentUser, 
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (newAppState.Route.HasNode(RouteDataNode.Admin,0))
		{
			newAppState = newAppState with
			{
				Pages = GetRegionComponentsMetaForContext(
					context: typeof(TfAdminPageScreenRegionContext),
					scope: null
				)
			};
		}
		else
		{
			newAppState = newAppState with
			{
				Pages = GetRegionComponentsMetaForContext(
					context: typeof(TfPageScreenRegionContext),
					scope: null
				)
			};
		}

		foreach (TfScreenRegionComponentMeta addonComponent in newAppState.Pages)
		{
			if (addonComponent.Type.ImplementsInterface(typeof(ITfAuxDataState)))
			{
				var component = (ITfAuxDataState)Activator.CreateInstance(addonComponent.Type);
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

	internal virtual ReadOnlyCollection<TfSpacePageAddonMeta> GetSpaceNodeComponents()
	{
		return _metaService.GetSpacePagesComponentsMeta();
	}
	
}
