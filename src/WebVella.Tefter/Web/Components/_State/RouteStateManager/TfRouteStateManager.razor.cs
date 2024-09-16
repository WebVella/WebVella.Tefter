namespace WebVella.Tefter.Web.Components;
public partial class TfRouteStateManager : FluxorComponent
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	protected override bool ShouldRender() => false;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_init(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_init(e.Location);
		StateHasChanged();
	}

	private void _init(string url)
	{
#if DEBUG
		Console.WriteLine($"================== TfRouteStateManager INIT  ================");
#endif
		Dispatcher.Dispatch(new SetRouteStateAction(
			component: this,
			state: Navigator.GetRouteState(url)
		));
	}

}
