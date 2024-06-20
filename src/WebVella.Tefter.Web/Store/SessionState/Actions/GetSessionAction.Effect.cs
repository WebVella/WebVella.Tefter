namespace WebVella.Tefter.Web.Store.SessionState;

public partial class SessionStateEffects
{
	[EffectMethod]
	public async Task HandleGetSessionAction(GetSessionAction action, IDispatcher dispatcher)
	{
		Result<UserSession> sessionResult = await tfService.GetUserSession(
			userId: action.UserId,
			spaceId: action.SpaceId,
			spaceDataId: action.SpaceDataId,
			spaceViewId: action.SpaceViewId
		);
		if (sessionResult.IsSuccess && sessionResult.Value is not null)
		{
			dispatcher.Dispatch(new InitSessionAction(
			spaceId: action.SpaceId,
			spaceDataId: action.SpaceDataId,
			spaceViewId: action.SpaceViewId,
			userSession: sessionResult.Value));
		}
	}

}

