namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceManageDetailsContentToolbar : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();

	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.SpaceUpdated -= On_SpaceUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.SpaceUpdated += On_SpaceUpdated;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async void On_SpaceUpdated(object? caller, TfSpace args)
	{
		await _init(TfAuthLayout.NavigationState);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_menu = new();
			if (navState.SpaceId is null)
				return;

			var space = TfUIService.GetSpace(navState.SpaceId.Value);
			if (space is null)
				return;

			var spacePages = TfUIService.GetSpacePages(space.Id);
			_menu.Add(new TfMenuItem
			{
				Url = string.Format(TfConstants.SpaceManagePageUrl, navState.SpaceId),
				Selected = navState.RouteNodes.Count == 3,
				IconCollapsed = TfConstants.GetIcon("Info"),
				Text = LOC("Details")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceManageAccessPageUrl, navState.SpaceId),
				Selected = navState.RouteNodes.Count == 4 && navState.RouteNodes[3] == RouteDataNode.Access,
				Text = LOC("Access"),
				IconCollapsed = TfConstants.GetIcon("Table"),
				BadgeContent = !space.IsPrivate
					? null
					: builder =>
					{
						builder.OpenComponent<FluentIcon<Icon>>(0);
						builder.AddAttribute(1, "Value", TfConstants.GetIcon("LockClosed", IconSize.Size16)!.WithColor(Color.Error));
						builder.CloseComponent();
					}
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceManagePagesPageUrl, navState.SpaceId, navState.SpaceViewId),
				Selected = navState.HasNode(RouteDataNode.Pages, 3),
				Text = LOC("Pages"),
				IconCollapsed = TfConstants.GetIcon("Document"),
				BadgeContent = spacePages.Count == 0
					? null
					: builder =>
					{
						builder.OpenElement(0, "span");
						builder.AddContent(1, spacePages.Count);
						builder.CloseElement();
					}
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