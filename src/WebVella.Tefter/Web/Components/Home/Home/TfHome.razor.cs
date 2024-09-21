
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Home.Home.TfHome", "WebVella.Tefter")]
public partial class TfHome : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

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