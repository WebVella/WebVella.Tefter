namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	[EffectMethod]
	public async Task HandleGetUserAction(GetUserAction action, IDispatcher dispatcher)
	{
		var user = await tfService.GetUserFromBrowserStorage();
		dispatcher.Dispatch(new InitUserAction(user));
	}

}

