namespace WebVella.Tefter.UI.Components;
public partial class TucAdminPageDetailsContent : TfBaseComponent, IDisposable
{
	private TfScreenRegionComponentMeta? _item = null;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(Navigator.GetRouteState());
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_item = null;
			if (navState.PageId is not null)
			{
				var pages = TfUIService.GetAddonAdminPages();
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