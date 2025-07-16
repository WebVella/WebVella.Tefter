namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceManageAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}


	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		try
		{
			_items = new();
			_items.Add(new TfMenuItem
			{
				Url = string.Format(TfConstants.SpaceManagePageUrl, navState.SpaceId),
				Selected = navState.RouteNodes.Count == 3,
				//Icon = new Icons.Regular.Size20.Info(),
				Text = LOC("Details")
			});
			_items.Add(new TfMenuItem
			{
				Url = string.Format(TfConstants.SpaceManagePagesPageUrl, navState.SpaceId),
				Selected = navState.RouteNodes.Count == 4 && navState.RouteNodes[3] == RouteDataNode.Pages,
				//Icon = new Icons.Regular.Size20.Table(),
				Text = LOC("Pages")
			});
			_items.Add(new TfMenuItem
			{
				Url = string.Format(TfConstants.SpaceManageAccessPageUrl, navState.SpaceId),
				Selected = navState.RouteNodes.Count == 4 && navState.RouteNodes[3] == RouteDataNode.Access,
				//Icon = new Icons.Regular.Size20.Table(),
				Text = LOC("Access")
			});
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}