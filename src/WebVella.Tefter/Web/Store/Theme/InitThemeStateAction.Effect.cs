namespace WebVella.Tefter.Web.Store.Base;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task InitThemeStateActionEffect(InitThemeStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SetThemeActionResult());
		return Task.CompletedTask;
	}
}