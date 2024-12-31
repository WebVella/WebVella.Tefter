using System.Diagnostics;

namespace WebVella.Tefter.Web.Components;
public partial class TfUserStateManager : FluxorComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] public TfUserEventProvider TfUserEventProvider { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] private UserStateUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

	private readonly AsyncLock locker = new AsyncLock();
	private Guid _renderedUserStateHash = Guid.Empty;
	private bool _isBusy = true;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			TfUserEventProvider.UserStateChanged -= On_UserStateChanged;
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
			TfUserEventProvider.UserStateChanged += On_UserStateChanged;
		}
	}

	private async Task _init(TfUserState state = null)
	{
		using (await locker.LockAsync())
		{
			if (state is null)
				state = await UC.InitUserState(ComponentId);
			var uri = new Uri(Navigator.Uri);
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
			else if (uri.LocalPath == "/" && !String.IsNullOrWhiteSpace(state.CurrentUser.Settings.StartUpUrl))
			{

				Navigator.NavigateTo(state.CurrentUser.Settings.StartUpUrl);
			}
			//Setup states
			Dispatcher.Dispatch(new SetUserStateAction(
				component: this,
				oldStateHash: TfUserState.Value.Hash,
				state: state with { Hash = Guid.NewGuid() }
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

	private void On_UserStateChanged(UserStateChangedEvent action)
	{
		if (action.State.SessionId == TfUserState.Value.SessionId) return;
		if (action.State.Hash == TfUserState.Value.Hash) return;

		var state = action.State with { Hash = action.State.Hash, SessionId = ComponentId };
		Debug.WriteLine($"++++++++++++ OUT SESSION: {ComponentId} ACTSESSION: {action.State.SessionId} HASH: {TfUserState.Value.Hash} ACTHASH: {action.State.Hash} ");
		Dispatcher.Dispatch(new SetUserStateAction(
			component: this,
			state: state,
			oldStateHash: TfUserState.Value.Hash,
			fromEvent:true
		));
		//StateHasChanged();
	}

}
