namespace WebVella.Tefter.Web.Components;
public partial class TfUserStateManager : FluxorComponent
{
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] private StateInitUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }


	public Guid _currentRenderLock = Guid.NewGuid();
	public Guid _oldRenderLock = Guid.Empty;

	protected override bool ShouldRender()
	{
		if (_currentRenderLock == _oldRenderLock) return false;

		base.ShouldRender();
		_oldRenderLock = _currentRenderLock;
#if DEBUG
		Console.WriteLine($"================== TfUserStateManager ReRender  ================");
#endif
		return true;
	}

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			var state = await UC.InitUserState();
			if (state.CurrentUser is null)
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
			UC.IsBusy = false;
			ActionSubscriber.SubscribeToAction<SetUserStateAction>(this, On_StateChanged);
			_currentRenderLock = Guid.NewGuid();
			await InvokeAsync(StateHasChanged);
		}
	}

	private void On_StateChanged(SetUserStateAction action)
	{
		_currentRenderLock = Guid.NewGuid();
		StateHasChanged();
	}

}
