namespace WebVella.Tefter.Web.Store.SessionState;

public partial class SessionStateEffects
{
	[EffectMethod]
	public async Task HandleGetSessionAction(GetSessionAction action, IDispatcher dispatcher)
	{
		var session = await tfService.GetUserSession(
			userId: action.UserId,
			spaceId: action.SpaceId,
			spaceDataId: action.SpaceDataId,
			spaceViewId: action.SpaceViewId
		);
		dispatcher.Dispatch(new InitSessionAction(session));
	}

}

