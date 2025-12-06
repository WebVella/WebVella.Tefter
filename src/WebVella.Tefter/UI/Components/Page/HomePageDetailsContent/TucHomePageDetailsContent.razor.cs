namespace WebVella.Tefter.UI.Components;
public partial class TucHomePageDetailsContent : TfBaseComponent, IDisposable
{
	private TfScreenRegionComponentMeta? _item = null;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_item = null;
			if (navState.PageId is not null)
			{
				var pages = TfMetaService.GetHomeAddonPages();
				_item = pages.FirstOrDefault(x => x.Id == navState.PageId);
			}
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}