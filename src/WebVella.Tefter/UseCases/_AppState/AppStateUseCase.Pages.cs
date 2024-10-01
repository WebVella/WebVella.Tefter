namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<TfAppState> InitPagesAsync(TucUser currentUser, TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (routeState.FirstNode == RouteDataFirstNode.Admin)
		{
			newAppState = newAppState with
			{
				Pages = GetAppPages(TfScreenRegion.AdminPages)
			};
		}
		else
		{
			newAppState = newAppState with
			{
				Pages = GetAppPages(TfScreenRegion.Pages)
			};
		}
		return Task.FromResult(newAppState);
	}

	internal List<TucScreenRegionComponentMeta> GetAppPages(TfScreenRegion? region = null)
	{
		var results = new List<TucScreenRegionComponentMeta>();
		var componentMeta = _screenRegionComponentManager.GetComponentMeta(region);
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

}
