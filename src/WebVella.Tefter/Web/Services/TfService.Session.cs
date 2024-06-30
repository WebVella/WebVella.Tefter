namespace WebVella.Tefter.Web.Services;
public partial interface ITfService
{
	Task<Result<UserSession>> GetUserSession(Guid userId, Guid? spaceId,
		Guid? spaceDataId, Guid? spaceViewId);


}

public partial class TfService : ITfService
{
	/// <summary>
	/// Get the complete user session for init of the UI
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="spaceId"></param>
	/// <param name="spaceDataId"></param>
	/// <param name="spaceViewId"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Result<UserSession>> GetUserSession(Guid userId, Guid? spaceId,
			Guid? spaceDataId, Guid? spaceViewId)
	{
		if (userId == Guid.Empty) throw new Exception("userId not provided");

		Result<User> userResult = await identityManager.GetUserAsync(userId);

		if (userResult.Value is null) throw new Exception("userId not found");

		var user = userResult.Value;
		Space space = null;
		SpaceData spaceData = null;
		SpaceView spaceView = null;

		string dataHashId = String.Empty;
		IDictionary<Guid, SpaceData> spaceDataDict = new Dictionary<Guid, SpaceData>();
		IDictionary<Guid, SpaceView> spaceViewDict = new Dictionary<Guid, SpaceView>();

		var userSpaces = await GetSpacesForUserAsync(userId);
		if (spaceId.HasValue)
		{
			var selSpace = userSpaces.FirstOrDefault(x => x.Id == spaceId.Value);
			if (selSpace is not null && selSpace.Data.Count > 0)
			{
				space = selSpace;
				spaceData = space.GetActiveData(spaceDataId);
				spaceView = spaceData.GetActiveView(spaceViewId);
			}
		}

		if (space is not null)
		{
			foreach (var sdi in space.Data)
			{
				spaceDataDict[sdi.Id] = sdi;
				foreach (var svi in sdi.Views)
				{
					spaceViewDict[svi.Id] = svi;
				}
			}
			dataHashId = ObjectHashHelper.CalculateHash(space);
		}

		var spaceNav = new List<MenuItem>();
		foreach (var item in userSpaces)
		{
			spaceNav.Add(new MenuItem
			{
				Icon = item.Icon,
				Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
				Match = NavLinkMatch.Prefix,
				Url = $"/space/{item.Id}",
				Title = item.Name,
				IconColor = item.Color,
			});
		}

		return Result.Ok(new UserSession
		{
			UserId = userId,
			ThemeColor = user.Settings.ThemeColor,
			ThemeMode = user.Settings.ThemeMode,
			SidebarExpanded = user.Settings.IsSidebarOpen,
			Space = space,
			SpaceData = spaceData,
			SpaceView = spaceView,
			DataHashId = dataHashId,
			SpaceDataDict = spaceDataDict,
			SpaceViewDict = spaceViewDict,
			SpaceNav = spaceNav,
			CultureCode = user.Settings.CultureName
		});
	}

	/// <summary>
	/// Sets changes of the User session in its UI part
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="themeMode"></param>
	/// <param name="themeColor"></param>
	/// <param name="sidebarExpanded"></param>
	/// <param name="cultureCode"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	
}
