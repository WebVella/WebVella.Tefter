namespace WebVella.Tefter.Web.Components;
public partial class TfRouteStateManager : FluxorComponent
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }

	private readonly AsyncLock locker = new AsyncLock();
	protected override bool ShouldRender() => false;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			using (await locker.LockAsync())
			{
				await _init(e.Location);
			}
		});
	}

	private Task _init(string url)
	{
		Dispatcher.Dispatch(new SetRouteStateAction(
			component: this,
			state: Navigator.GetRouteState(url)
		));
		return Task.CompletedTask;
	}

}
