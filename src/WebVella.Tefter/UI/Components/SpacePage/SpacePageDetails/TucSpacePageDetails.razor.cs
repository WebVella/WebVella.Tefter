namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageDetails : TfBaseComponent, IDisposable
{
	private bool _isRemoving = false;
	private TfSpacePage? _spacePage = null;
	private TfSpace? _space = null;
	public TfNavigationState? _navState = null;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.SpaceUpdated -= On_SpaceUpdated;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.SpaceUpdated += On_SpaceUpdated;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async void On_SpaceUpdated(object? caller, TfSpace args)
	{
		await _init();
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			_navState = await TfUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			if (_navState.SpacePageId.HasValue && _spacePage?.Id != _navState.SpacePageId)
			{
				_spacePage = TfUIService.GetSpacePage(_navState.SpacePageId.Value);
				_space = null;
				if (_spacePage is not null)
					_space = TfUIService.GetSpace(_spacePage.SpaceId);
			}
		}
		finally
		{
			UriInitialized = _navState.Uri;
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
				Mode = TfComponentMode.Read,
				SpacePage = _spacePage,
				Space = _space,
				CurrentUser = TfAuthLayout.CurrentUser,
				EditNode = EventCallback.Factory.Create(this, _onEdit),
				DeleteNode = EventCallback.Factory.Create(this, _onRemove)
			};
		}


		return dict;
	}

	private async Task _onRemove()
	{
		if (_spacePage is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space page removed?")))
			return;

		if (_isRemoving) return;

		_isRemoving = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			TfUIService.DeleteSpacePage(_spacePage);
			ToastService.ShowSuccess(LOC("Space page deleted!"));
			var spacePages = TfUIService.GetSpacePages(_spacePage.SpaceId);
			Guid? firstPageId = null;
			foreach (var page in spacePages)
			{
				var pageId = page.GetFirstNavigatedPageId();
				if (pageId is not null)
				{
					firstPageId = pageId;
					break;
				}
			}
			if (firstPageId is not null)
				Navigator.NavigateTo(string.Format(TfConstants.SpacePagePageUrl, _spacePage.SpaceId, firstPageId.Value));
			else
				Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isRemoving = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _onEdit()
	{
		if (_spacePage is null) return;

		var spPage = TfUIService.GetSpacePage(_spacePage.Id);
		if (spPage == null)
		{
			ToastService.ShowError(LOC("Space page not found"));
			return;
		}
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
		spPage,
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
			ToastService.ShowSuccess(LOC("Space page successfully saved!"));
		}
	}

}