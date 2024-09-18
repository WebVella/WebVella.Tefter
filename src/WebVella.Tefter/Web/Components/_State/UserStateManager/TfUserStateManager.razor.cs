namespace WebVella.Tefter.Web.Components;
public partial class TfUserStateManager : FluxorComponent
{
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] private UserStateUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }

	private readonly AsyncLock locker = new AsyncLock();
	private bool _isBusy = true;
	protected override bool ShouldRender()
	{
		Console.WriteLine($"*-ShouldRender********************* {DateTime.Now}");
		//if (_renderedStateHash == TfUserState.Value.Hash) return false;

		//base.ShouldRender();
		//_renderedStateHash = TfUserState.Value.Hash;
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
			Console.WriteLine($"*-OnAfterRenderAsync********************");
			await _init();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
			ActionSubscriber.SubscribeToAction<SetUserStateAction>(this, On_StateChanged);
		}
	}

	private async Task _init()
	{
		Console.WriteLine($"*-_init*********************");
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
			Console.WriteLine($"*-_init*********************");
		}
	}

	private void On_StateChanged(SetUserStateAction action)
	{
		if (action.StateComponent == this) return;
		Console.WriteLine($"1-On_StateChanged******************");
		InvokeAsync(async () =>
		{
			await _init();
			await InvokeAsync(StateHasChanged);
		});
	}

}
