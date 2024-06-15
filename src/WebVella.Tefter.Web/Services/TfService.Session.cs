using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Services;
public partial interface ITfService
{
	ValueTask<UserSession> GetUserSession(Guid userId, Guid? spaceId,
		Guid? spaceDataId, Guid? spaceViewId);
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
		List<Space> spaceList = new();
		IDictionary<Guid, Space> spaceDict = new Dictionary<Guid, Space>();
		List<SpaceData> spaceDataList = new();
		IDictionary<Guid, SpaceData> spaceDataDict = new Dictionary<Guid, SpaceData>();
		List<SpaceView> spaceViewList = new();
		IDictionary<Guid, SpaceView> spaceViewDict = new Dictionary<Guid, SpaceView>();

		var userSpaces = await GetSpacesForUserAsync(userId);

		foreach (var si in userSpaces)
		{
			spaceList.Add(si);
			spaceDict[si.Id] = si;
			foreach (var sdi in si.Data)
			{
				spaceDataList.Add(sdi);
				spaceDataDict[sdi.Id] = sdi;
				foreach (var svi in sdi.Views)
				{
					spaceViewList.Add(svi);
					spaceViewDict[svi.Id] = svi;
				}
			}
		}
		dataHashId = ObjectHashHelper.CalculateHash(spaceList);


		if (spaceId is null)
		{
			//Preset the default spaces for user if any exist
			if (userSpaces.Count > 0 && userSpaces[0].Data.Count > 0)
			{
				space = userSpaces[0];
				spaceData = space.GetActiveData(null);
				spaceView = spaceData.GetActiveView(null);
			}
		}
		else
		{
			var selSpace = userSpaces.FirstOrDefault(x => x.Id == spaceId.Value);
			if (selSpace is not null && selSpace.Data.Count > 0)
			{
				space = selSpace;
				spaceData = space.GetActiveData(spaceDataId);
				spaceView = spaceData.GetActiveView(spaceViewId);
			}
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
			SpaceDataList = spaceDataList,
			SpaceDict = spaceDict,
			SpaceList = spaceList,
			SpaceViewDict = spaceViewDict,
			SpaceViewList = spaceViewList,
		};
	}

}
