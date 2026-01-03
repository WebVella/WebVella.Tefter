namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceManageDetailsContentToolbar : TfBaseComponent, IAsyncDisposable
{
	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();
	private IAsyncDisposable _spaceUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spaceUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spaceUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceUpdatedEventPayload>(
			handler: On_SpaceUpdatedEventAsync);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpaceUpdatedEventAsync(string? key, TfSpaceUpdatedEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_menu = new();
			if (navState.SpaceId is null)
				return;

			var space = TfService.GetSpace(navState.SpaceId.Value);
			if (space is null)
				return;

			var spacePages = TfService.GetSpacePages(space.Id);
			_menu.Add(new TfMenuItem
			{
				Url = string.Format(TfConstants.SpaceManagePageUrl, navState.SpaceId)
					.GenerateWithLocalAndQueryAsReturnUrl(navState.ReturnUrl),
				Selected = navState.RouteNodes.Count == 3,
				IconCollapsed = TfConstants.GetIcon("Info"),
				Text = LOC("Details")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceManageAccessPageUrl, navState.SpaceId)
					.GenerateWithLocalAndQueryAsReturnUrl(navState.ReturnUrl),
				Selected = navState.RouteNodes is [_, _, _, RouteDataNode.Access],
				Text = LOC("Access"),
				IconCollapsed = TfConstants.GetIcon("Table"),
				BadgeContent = !space.IsPrivate
					? null
					: builder =>
					{
						builder.OpenComponent<FluentIcon<Icon>>(0);
						builder.AddAttribute(1, "Value",
							TfConstants.GetIcon("LockClosed", IconSize.Size16)!.WithColor(Color.Error));
						builder.CloseComponent();
					}
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceManagePagesPageUrl, navState.SpaceId, navState.SpaceViewId)
					.GenerateWithLocalAndQueryAsReturnUrl(navState.ReturnUrl),
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