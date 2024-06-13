namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	[EffectMethod]
	public async Task HandleLoginUserAction(LogoutUserAction action, IDispatcher dispatcher)
	{
		await tfService.LogoutUser();
		dispatcher.Dispatch(new InitUserAction(null));
	}

}

