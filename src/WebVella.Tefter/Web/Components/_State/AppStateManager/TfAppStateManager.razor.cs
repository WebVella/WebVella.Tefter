namespace WebVella.Tefter.Web.Components;
public partial class TfAppStateManager : FluxorComponent
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] private IState<TfUserState> TfUserState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			await _init(null);
			Navigator.LocationChanged += Navigator_LocationChanged;
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _init(string url)
	{
		if (TfUserState.Value.CurrentUser is null) return;
		var state = await UC.InitState(TfUserState.Value.CurrentUser, url);
		Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: state
		));
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			await _init(e.Location);
			await InvokeAsync(StateHasChanged);
		});


	}

}
