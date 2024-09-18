
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Dashboard.TfDashboard", "WebVella.Tefter")]
public partial class TfDashboard : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private int counterINT = 0;
	private int counter = 0;

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		counterINT++;
		Console.WriteLine($"+****************** OnInitialized {counterINT} {TfRouteState.Value.SpaceId} {TfAppState.Value.SpaceViewList.Count}");
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		counter++;
		Console.WriteLine($"+****************** TfDashboard Render {counter}");
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