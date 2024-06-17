using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Services;
public partial interface ITfService
{
	ValueTask<UserSession> GetUserSession(Guid userId, Guid? spaceId,
		Guid? spaceDataId, Guid? spaceViewId);
	Task SetSessionUI(Guid userId, DesignThemeModes themeMode, OfficeColor themeColor, bool sidebarExpanded);
}

public partial class TfService : ITfService
{
	public async ValueTask<UserSession> GetUserSession(Guid userId, Guid? spaceId,
	Guid? spaceDataId, Guid? spaceViewId)
	{
		if (userId == Guid.Empty) throw new Exception("userId not provided");

		var user = await GetUserById(userId);
		if (user is null) throw new Exception("userId not found");

		Space space = null;
		SpaceData spaceData = null;
		SpaceView spaceView = null;

		string dataHashId = String.Empty;
		IDictionary<Guid, SpaceData> spaceDataDict = new Dictionary<Guid, SpaceData>();
		IDictionary<Guid, SpaceView> spaceViewDict = new Dictionary<Guid, SpaceView>();

		var userSpaces = await GetSpacesForUserAsync(userId);
		if(spaceId.HasValue)
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

		return new UserSession
		{
			ThemeColor = user.ThemeColor,
			ThemeMode = user.ThemeMode,
			SidebarExpanded = user.SidebarExpanded,
			Space = space,
			SpaceData = spaceData,
			SpaceView = spaceView,
			DataHashId = dataHashId,
			SpaceDataDict = spaceDataDict,
			SpaceViewDict = spaceViewDict,
			SpaceNav = spaceNav,
		};
	}

	public async Task SetSessionUI(Guid userId, DesignThemeModes themeMode, OfficeColor themeColor, bool sidebarExpanded){ 
		await dataBroker.SetUserUISettingsAsync(userId, themeMode, themeColor, sidebarExpanded);
	}

}
