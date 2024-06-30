namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task SetSidebarActionEffect(SetSidebarAction action, IDispatcher dispatcher)
	{
		var setResult = await UseCase.SetSidebarState(
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