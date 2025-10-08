namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageManageContent : TfBaseComponent, IDisposable
{
	private bool _isDeleting = false;
	private TfSpace _space = null!;
	private TfSpacePage _spacePage = null!;
	private TfSpacePageAddonMeta? _component = null;
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
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
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
			var pageMeta = TfMetaService.GetSpacePagesComponentsMeta();
			_component = pageMeta.FirstOrDefault(x => x.Instance.AddonId == _spacePage.ComponentId);			
			
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editPage()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			_spacePage,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
		}
	}

	private async Task _deletePage()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this page deleted?")))
			return;
		
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			TfService.DeleteSpacePage(_spacePage);
			var pages = TfService.GetSpacePages(_space.Id);
			ToastService.ShowSuccess(LOC("Page was successfully deleted"));
			if (pages.Count > 0)
			{
				Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl, _space.Id, pages[0].Id));
			}
			else
			{
				Navigator.NavigateTo(TfConstants.HomePageUrl);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isDeleting = false;
			await InvokeAsync(StateHasChanged);
		}

	}
}
