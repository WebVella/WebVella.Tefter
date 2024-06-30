namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task SetCultureActionEffect(SetCultureAction action, IDispatcher dispatcher)
	{
		var result = await UseCase.SetUserCulture(
			userId: action.UserId,
			cultureCode: action.Culture.CultureCode
		);
		if(result.IsFailed) return;
		dispatcher.Dispatch(new SetCultureActionResult());
	}
}