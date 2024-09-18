namespace WebVella.Tefter.Web.Components;
public partial class TfUserStateManager : FluxorComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] private UserStateUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }

	private readonly AsyncLock locker = new AsyncLock();
	private Guid _renderedUserStateHash = Guid.Empty;
	private bool _isBusy = true;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override bool ShouldRender()
	{
		if (_renderedUserStateHash == TfUserState.Value.Hash) return false;
		_renderedUserStateHash = TfUserState.Value.Hash;
		base.ShouldRender();
		return true;
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			await _init();
			_isBusy = false;
			_renderedUserStateHash = Guid.NewGuid();//to force rerender
			await InvokeAsync(StateHasChanged);
			ActionSubscriber.SubscribeToAction<SetUserStateAction>(this, On_StateChanged);
		}
	}

	private async Task _init()
	{
		using (await locker.LockAsync())
		{
			var state = await UC.InitUserState();
			if (state is null || state.CurrentUser is null)
			{
				Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
				return;
			}
			else if (state.Culture is null)
			{
				Navigator.ReloadCurrentUrl();
				return;
			}
			//Setup states
			Dispatcher.Dispatch(new SetUserStateAction(
				component: this,
				state: state
			));
		}
	}

	private void On_StateChanged(SetUserStateAction action)
	{
		if (action.StateComponent == this) return;
		InvokeAsync(async () =>
		{
			await InvokeAsync(StateHasChanged);
		});
	}

}
