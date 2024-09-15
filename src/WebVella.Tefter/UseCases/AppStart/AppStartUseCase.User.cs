namespace WebVella.Tefter.UseCases.AppStart;

internal partial class AppStartUseCase
{
	internal async Task<TucUser> InitUserAsync()
	{
		var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
		//Temporary fix for multitab logout- we check the cookie as well
		var cookie = await new CookieService(_jsRuntime).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);
		if (cookie is null || user.Identity is null || !user.Identity.IsAuthenticated ||
			(user.Identity as TfIdentity) is null ||
			(user.Identity as TfIdentity).User is null)
		{
			_navigationManager.NavigateTo(TfConstants.LoginPageUrl, true);
			return null;
		}
		var tfUser = ((TfIdentity)user.Identity).User;
		if (tfUser is null) return null;

		return new TucUser(tfUser);
	}

	internal Task<List<TucSpace>> InitUserSpaces(TucUser user)
	{

		var userSpaces = _spaceManager.GetSpacesListForUser(user.Id).Value.Select(s => new TucSpace(s)).OrderBy(x => x.Position).ToList();
		var spacesHS = userSpaces.Select(x => x.Id).Distinct().ToHashSet();
		var allViews = _spaceManager.GetAllSpaceViews().Value;
		var spaceViewsDict = new Dictionary<Guid, List<TfSpaceView>>();
		foreach (var item in allViews)
		{
			if (!spacesHS.Contains(item.SpaceId)) continue;
			if (!spaceViewsDict.ContainsKey(item.SpaceId)) spaceViewsDict[item.SpaceId] = new();
			spaceViewsDict[item.SpaceId].Add(item);
		}

		foreach (var spaceId in spaceViewsDict.Keys)
		{
			spaceViewsDict[spaceId] = spaceViewsDict[spaceId].OrderBy(x => x.Position).ToList();
		}

		foreach (var space in userSpaces)
		{
			if (spaceViewsDict.ContainsKey(space.Id) && spaceViewsDict[space.Id].Count > 0)
				space.DefaultViewId = spaceViewsDict[space.Id][0].Id;
		}
		return Task.FromResult(userSpaces);

	}

}
