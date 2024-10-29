
namespace WebVella.Tefter.Web.Components;
public partial class TfPageLoader : ComponentBase, IAsyncDisposable
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }

	private bool _isRouteChanging = true;

	public ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= Navigator_LocationChanged;
		
		return ValueTask.CompletedTask;
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if(firstRender) {
			Navigator.LocationChanged += Navigator_LocationChanged;
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_StateChanged);
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_isRouteChanging = true;
		StateHasChanged();
	}

	private void On_StateChanged(SetAppStateAction action)
	{
		_isRouteChanging = false;
		StateHasChanged();
	}
}