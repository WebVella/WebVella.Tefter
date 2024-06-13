namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	[EffectMethod]
	public async Task HandleLoginUserAction(LoginUserAction action, IDispatcher dispatcher)
	{
		var user = await tfService.LoginUserByEmailAndPassword(action.Email,action.Password);
		dispatcher.Dispatch(new InitUserAction(user));
	}

}

