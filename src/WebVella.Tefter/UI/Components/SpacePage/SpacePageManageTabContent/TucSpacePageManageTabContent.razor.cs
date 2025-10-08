namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageManageTabContent : TfBaseComponent, IDisposable
{
	private bool _isDeleting = false;
	private TfSpace _space = null!;
	private TfSpacePage _spacePage = null!;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.SpacePageUpdatedEvent -= On_SpacePageChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpacePageUpdatedEvent += On_SpacePageChanged;
	}
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpacePageChanged(object args)
	{
		await _init(TfAuthLayout.GetState().NavigationState);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			if (navState.SpaceId is null)
				throw new Exception("Space Id not found in URL");
			_space = TfService.GetSpace(navState.SpaceId.Value);
			if (navState.SpacePageId is null)
				throw new Exception("Page Id not found in URL");
			_spacePage = TfService.GetSpacePage(navState.SpacePageId.Value);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		if (_spacePage is not null)
		{
			dict["Context"] = new TfSpacePageAddonContext
			{
				ComponentOptionsJson = _spacePage.ComponentOptionsJson,
				Icon = _spacePage.FluentIconName,
				Mode = TfComponentMode.Manage,
				SpacePage = _spacePage,
				Space = _space,
				CurrentUser = TfAuthLayout.GetState().User,
			};
		}


		return dict;
	}
}
