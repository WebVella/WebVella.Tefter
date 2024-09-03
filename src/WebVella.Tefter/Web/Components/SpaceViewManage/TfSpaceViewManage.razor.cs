namespace WebVella.Tefter.Web.Components.SpaceViewManage;
public partial class TfSpaceViewManage : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	[Inject] private SpaceUseCase UC { get; set; }

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await UC.InitSpaceViewManageAfterRender(SpaceState.Value);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private void On_StateChanged(SpaceStateChangedAction action)
	{
		InvokeAsync(async () =>
		{
			await UC.InitSpaceViewManageAfterRender(SpaceState.Value);
			await InvokeAsync(StateHasChanged);
		});

	}

}