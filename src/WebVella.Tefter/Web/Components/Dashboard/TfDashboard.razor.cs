
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Dashboard.TfDashboard","WebVella.Tefter")]
public partial class TfDashboard : TfBaseComponent
{
	[Inject] private DashboardUseCase UC { get; set; }
	[Inject] protected IState<DashboardState> DashboardState { get; set; }


	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		ActionSubscriber.SubscribeToAction<DashboardStateChangedAction>(this, On_DashboardStateChangedAction);
	}

	private void On_DashboardStateChangedAction(DashboardStateChangedAction action)
	{
		base.InvokeAsync(async () =>
		{
			UC.IsBusy = DashboardState.Value.IsBusy;
			await InvokeAsync(StateHasChanged);
		});

	}


	void AddInNotificationCenter()
	{
		MessageService.ShowMessageBar(options =>
		{
			options.Intent = MessageIntent.Error;
			options.Title = $"Simple message";
			options.Body = "<ul><li>test</li><li>test 2</li></ul>";
			options.Timestamp = DateTime.Now;
			options.Timeout = 5000;
			options.Section = TfConstants.MESSAGES_NOTIFICATION_CENTER;
		});
	}
}