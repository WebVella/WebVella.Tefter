namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task SetCultureActionEffect(SetCultureAction action, IDispatcher dispatcher)
	{
		action.UseCase.SetUserCulture(
			userId: action.UserId,
			cultureCode: action.Culture.CultureCode
		);
		dispatcher.Dispatch(new SetCultureActionResult());
		return Task.CompletedTask;
	}
}