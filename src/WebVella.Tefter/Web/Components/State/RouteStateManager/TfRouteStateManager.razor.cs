namespace WebVella.Tefter.Web.Components;
public partial class TfRouteStateManager : FluxorComponent
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }

	private readonly AsyncLock locker = new AsyncLock();
	protected override bool ShouldRender() => false;
	private IDisposable navigationChangingRegistration;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			navigationChangingRegistration?.Dispose();
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(null);
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		Navigator.LocationChanged += Navigator_LocationChanged;
		//navigationChangingRegistration = Navigator.RegisterLocationChangingHandler(LocationChangingHandler);
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			await _init(e.Location);
		});
	}

	private async ValueTask LocationChangingHandler(LocationChangingContext  args)
	{
		await _init(args.TargetLocation);
	}

	private async Task _init(string url)
	{
		if(String.IsNullOrWhiteSpace(url)) return;
		using (await locker.LockAsync())
		{
			Dispatcher.Dispatch(new SetRouteStateAction(
				component: this,
				state: Navigator.GetRouteState(url)
			));
		}
	}

}
