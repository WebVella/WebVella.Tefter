using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.UseCases.AppStart;

internal partial class AppStateUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly NavigationManager _navigationManager;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<AppStateUseCase> LOC;


	public AppStateUseCase(
		AuthenticationStateProvider authenticationStateProvider,
		IJSRuntime jsRuntime,
		IIdentityManager identityManager,
		ITfSpaceManager spaceManager,
		NavigationManager navigationManager,
		IToastService toastService,
		IMessageService messageService,
		IStringLocalizer<AppStateUseCase> loc
		)
	{
		_authenticationStateProvider = authenticationStateProvider;
		_jsRuntime = jsRuntime;
		_identityManager = identityManager;
		_spaceManager = spaceManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		LOC = loc;
	}

	internal bool IsBusy { get; set; } = true;

	internal async Task<TfAppState> InitState(TucUser currentUser, string url)
	{
		var result = new TfAppState();
		var routeState = _navigationManager.GetRouteState(url);

		#region << Admin users >>
		{
			if (routeState.FirstNode == RouteDataFirstNode.Admin
			&& routeState.SecondNode == RouteDataSecondNode.Users)
			{
				if (result.AdminUsers.Count == 0) //Fill in only if not already loaded
					result = result with { AdminUsers = await GetUsersAsync(null, 1, TfConstants.PageSize), AdminUsersPage = 2 };

				if (routeState.UserId.HasValue)
				{
					var adminUser = await GetUser(routeState.UserId.Value);
					result = result with { AdminManagedUser = adminUser };
					if (adminUser is not null)
					{
						result = result with { AdminManagedUser = adminUser };
						if (!result.AdminUsers.Any(x => x.Id == adminUser.Id))
						{
							var users = result.AdminUsers.ToList();
							users.Add(adminUser);
							result = result with { AdminUsers = users };
						}

						var roles = await GetUserRoles();
						result = result with { UserRoles = roles ?? new List<TucRole>() };

						//check for the other tabs
						if (routeState.ThirdNode == RouteDataThirdNode.Access)
						{
						}
						else if (routeState.ThirdNode == RouteDataThirdNode.Saves)
						{
						}
					}
				}
			}
			else
			{
				result = result with { AdminUsers = new(), AdminUsersPage = 1 };
			}
		}
		#endregion


		#region << User spaces >>
		{
			if (routeState.FirstNode == RouteDataFirstNode.Home
			|| routeState.FirstNode == RouteDataFirstNode.FastAccess
			|| routeState.FirstNode == RouteDataFirstNode.Space)
			{
				if (result.CurrentUserSpaces.Count == 0) //Fill in only if not already loaded
					result = result with { CurrentUserSpaces = await GetUserSpaces(currentUser) };
			}
			else
			{
				result = result with { CurrentUserSpaces = new() };
			}
		}
		#endregion

		return result;
	}
	internal Task<List<TucSpace>> GetUserSpaces(TucUser user)
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
