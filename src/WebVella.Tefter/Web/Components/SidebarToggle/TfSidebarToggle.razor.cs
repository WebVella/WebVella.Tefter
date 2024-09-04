namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SidebarToggle.TfSidebarToggle","WebVella.Tefter")]
public partial class TfSidebarToggle : TfBaseComponent
{
	[Inject] protected IStateSelection<UserState, Guid> UserIdState { get; set; }
	[Inject] protected IState<ScreenState> ScreenState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenSidebarState { get; set; }
	[Inject] private IKeyCodeService KeyCodeService { get; set; }

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
		ScreenSidebarState.Select(x => x?.SidebarExpanded ?? true);
		UserIdState.Select(x => x?.User?.Id ?? Guid.Empty);
		KeyCodeService.RegisterListener(OnKeyDownAsync);
	}


	private void _toggle()
	{
		Dispatcher.Dispatch(new SetSidebarAction(
			userId: UserIdState.Value,
			sidebarExpanded: !ScreenSidebarState.Value));
	}

	public Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.CtrlKey && args.Key == KeyCode.Function11)
			_toggle();

		return Task.CompletedTask;
	}
}