namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task SetThemeActionEffect(SetThemeAction action, IDispatcher dispatcher)
	{

		var result = await UseCase.SetUserTheme(
			userId: action.UserId,
			themeMode: action.ThemeMode,
			themeColor: action.ThemeColor
		);
		ResultUtils.ProcessServiceResponse(
			response: result,
			toastErrorMessage: "",
			notificationErrorTitle: "",
			toastService: ToastService,
			messageService: MessageService
		);
		if (result.IsFailed) return;
		dispatcher.Dispatch(new SetThemeActionResult());
	}
}