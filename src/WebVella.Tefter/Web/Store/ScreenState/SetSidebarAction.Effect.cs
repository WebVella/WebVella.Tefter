namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task SetSidebarActionEffect(SetSidebarAction action, IDispatcher dispatcher)
	{
		var result = await UseCase.SetSidebarState(
					userId: action.UserId,
					sidebarExpanded: action.SidebarExpanded
				);

		ResultUtils.ProcessServiceResult(
			result: result,
			toastErrorMessage: "",
			notificationErrorTitle: "",
			toastService: ToastService,
			messageService: MessageService
		);
		if (result.IsFailed) return;
		dispatcher.Dispatch(new SetSidebarActionResult());
	}
}