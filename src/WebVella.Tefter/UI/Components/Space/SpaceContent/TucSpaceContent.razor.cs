namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceContent : TfBaseComponent, IDisposable
{
	private TfSpace _space = null!;
	private TfNavigationState _navState = null!;
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
		_navState = navState;

		try
		{
			if (_navState.SpaceId is null) return;
			_space = TfService.GetSpace(_navState.SpaceId.Value);
			if (_space is null) return;

			var spacePages = TfService.GetSpacePages(_space.Id);
			if(spacePages.Count > 0)
				Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl,_space.Id, spacePages[0].Id));
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _addPage()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			new TfSpacePage() { SpaceId = TfAuthLayout.GetState().Space!.Id },
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var page = (TfSpacePage)result.Data;
			Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl,page.SpaceId,page.Id));			
		}
	}	
	
}