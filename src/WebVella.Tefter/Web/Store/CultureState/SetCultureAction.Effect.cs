namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task SetCultureActionEffect(SetCultureAction action, IDispatcher dispatcher)
	{
		if (!action.Persist)
		{
			dispatcher.Dispatch(new SetCultureActionResult());
			return;
		}

		var setResult = await TefterService.SetUserCulture(
					userId: action.UserId,
					cultureCode: action.Culture.CultureCode
				);

		if (setResult.IsSuccess && setResult.Value)
		{
			//if persist success do something
		}
		else
		{
			Console.WriteLine($"Persisting SetCultureAction failed");
		}
		dispatcher.Dispatch(new SetCultureActionResult());
	}
}