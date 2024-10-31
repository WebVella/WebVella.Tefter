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
				Pages = GetAddonComponents(TfScreenRegion.AdminPages)
			};
		}
		else
		{
			newAppState = newAppState with
			{
				Pages = GetAddonComponents(TfScreenRegion.Pages)
			};
		}

		foreach (TucScreenRegionComponentMeta addonComponent in newAppState.Pages)
		{
			if (addonComponent.ComponentType is not null
				&& addonComponent.ComponentType.GetInterface(nameof(ITucAuxDataUseComponent)) != null)
			{
				var component = (ITucAuxDataUseComponent)Activator.CreateInstance(addonComponent.ComponentType);
				component.OnSpaceViewStateInited(
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

	internal List<TucScreenRegionComponentMeta> GetAddonComponents(TfScreenRegion? region = null)
	{
		var results = new List<TucScreenRegionComponentMeta>();
		var componentMeta = _metaProvider.GetScreenRegionComponentsMeta(region);
		foreach (var meta in componentMeta)
		{
			results.Add(new TucScreenRegionComponentMeta
			{
				Region = meta.ScreenRegion,
				Position = meta.Position,
				Slug = meta.UrlSlug,
				Name = meta.Name,
				ComponentType = meta.ComponentType,
			});
		}
		return results.OrderBy(x => x.Position).ThenBy(x => x.Name).ToList();
	}

	internal ReadOnlyCollection<TfSpaceNodeComponentMeta> GetSpaceNodeComponents()
	{
		return _metaProvider.GetSpaceNodesComponentsMeta();
	}
	
}
