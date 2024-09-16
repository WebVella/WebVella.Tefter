namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SidebarToggle.TfSidebarToggle", "WebVella.Tefter")]
public partial class TfSidebarToggle : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] private IKeyCodeService KeyCodeService { get; set; }
	[Inject] private StateEffectsUseCase UC { get; set; }

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			KeyCodeService.UnregisterListener(OnKeyDownAsync);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		KeyCodeService.RegisterListener(OnKeyDownAsync);
	}


	private async Task _toggle()
	{
		try
		{
			var resultSrv = await UC.SetSidebarState(
						userId: TfUserState.Value.CurrentUser.Id,
						sidebarExpanded: !TfUserState.Value.SidebarExpanded);

			ProcessServiceResponse(resultSrv);

			if (resultSrv.IsSuccess)
			{
				Dispatcher.Dispatch(new SetUserStateAction(
					component: this,
					state: TfUserState.Value with { CurrentUser = resultSrv.Value }));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}


	}

	public async Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.CtrlKey && args.Key == KeyCode.Function11) await _toggle();
	}
}