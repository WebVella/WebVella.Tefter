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

		Space space = null;
		SpaceData spaceData = null;
		SpaceView spaceView = null;
		var userSpaces = await GetSpacesForUserAsync(userId);
		if (spaceId is null)
		{
			//Preset the default spaces for user if any exist
			if (userSpaces.Count > 0 && userSpaces[0].DataItems.Count > 0)
			{
				space = userSpaces[0];
				spaceData = space.DataItems[0];
				spaceView = spaceData.GetActiveView(null);
			}
		}
		else{ 
			var selSpace = userSpaces.FirstOrDefault(x=> x.Id == spaceId.Value);
			if(selSpace is not null && selSpace.DataItems.Count > 0){ 
				space = selSpace;
				if(spaceDataId is not null){ 
					spaceData = space.DataItems.FirstOrDefault(x=> x.Id == spaceDataId.Value);
				}

				if(spaceDataId is null){ 
					spaceData = space.DataItems[0];
				}

				spaceView = spaceData.GetActiveView(spaceViewId);
			}
		}


		return new UserSession
		{
			Space = space,
			SpaceData = spaceData,
			SpaceView = spaceView,
		};
	}

}
