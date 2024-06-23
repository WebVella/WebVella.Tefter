namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task SetSidebarActionEffect(SetSidebarAction action, IDispatcher dispatcher)
	{
		if (!action.Persist)
		{
			dispatcher.Dispatch(new SetSidebarActionResult());
			return;
		}
		var setResult = await TefterService.SetUserSidebarExpanded(
					userId: action.UserId,
					sidebarExpanded: action.SidebarExpanded
				);

		if (setResult.IsSuccess && setResult.Value)
		{
			//Do something on success
		}
		else
		{
			Console.WriteLine($"Persisting SetSidebarAction failed");
		}
		dispatcher.Dispatch(new SetSidebarActionResult());
	}
}