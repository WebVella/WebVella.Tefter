namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitPagesAsync(TucUser currentUser, TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (routeState.FirstNode == RouteDataFirstNode.Admin)
		{
			newAppState = newAppState with
			{
				Pages = new()
			};
			return newAppState;
		}
		newAppState = newAppState with
		{
			Pages = GetAppPages()
		};


		return newAppState;
	}

	internal List<TucScreenRegionComponentMeta> GetAppPages()
	{
		var results = new List<TucScreenRegionComponentMeta>();
		var componentMeta = _screenRegionComponentManager.GetComponentMeta(ScreenRegion.Pages);
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
