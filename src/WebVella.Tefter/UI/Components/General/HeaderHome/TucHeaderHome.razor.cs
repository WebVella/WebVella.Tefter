namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderHome : TfBaseComponent, IDisposable
{

	private IReadOnlyDictionary<string, object>? _attributes = null;

	private List<TfMenuItem> _menu = new();

	public void Dispose()
	{
		TfEventProvider.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(TfNavigationStateChangedEvent args)
	{
		if (args.IsUserApplicable(TfAuthLayout.CurrentUser) && UriInitialized != args.Payload.Uri)
			await _init(args.Payload);
	}

	private async Task _init(TfNavigationState navState)
	{
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
				Tooltip = LOC("Home"),
				IconCollapsed = TfConstants.GetIcon("Home"),
				IconExpanded = TfConstants.GetIcon("Home"),
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