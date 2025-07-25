namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderHome : TfBaseComponent, IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private IReadOnlyDictionary<string, object>? _attributes = null;

	private List<TfMenuItem> _menu = new();

	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
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

		_menu = new();
		try
		{
			_attributes = (new Dictionary<string, object>(){
			{"title",LOC("Browse Spaces")}
			}).AsReadOnly();

			_menu.Add(new TfMenuItem
			{
				Id = (new Guid("176c9d30-58bb-4ff9-8101-ba90252147f4")).ToString(),
				Text = null,
				Url = "/",
				Tooltip = LOC(TfConstants.HomeMenuTitle),
				IconCollapsed = TfConstants.HomeIcon,
				IconExpanded = TfConstants.HomeIcon,
				Selected = navState.RouteNodes.Count == 1 && navState.RouteNodes[0] == RouteDataNode.Home,
			});
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}


}