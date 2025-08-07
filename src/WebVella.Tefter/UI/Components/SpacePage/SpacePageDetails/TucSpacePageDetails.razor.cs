namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpacePage.SpacePageDetails.TucSpacePageDetails", "WebVella.Tefter")]
public partial class TucSpacePageDetails : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isRemoving = false;
	private TfSpacePage? _spacePage = null;
	public TfNavigationState? _navState = null;
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfSpaceUIService.SpaceUpdated -= On_SpaceUpdated;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfSpaceUIService.SpaceUpdated += On_SpaceUpdated;
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
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			if (_navState.SpacePageId.HasValue &&
			(_spacePage is null || _spacePage.Id == _navState.SpacePageId))
			{
				_spacePage = TfSpaceUIService.GetSpacePage(_navState.SpacePageId.Value);
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
				SpaceId = _spacePage.SpaceId,
				SpacePageId = _spacePage.Id,
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
			TfSpaceUIService.DeleteSpacePage(_spacePage);
			ToastService.ShowSuccess(LOC("Space page deleted!"));
			var spacePages = TfSpaceUIService.GetSpacePages(_spacePage.SpaceId);
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
				Navigator.NavigateTo(string.Format(TfConstants.SpacePagePageUrl, _spacePage.Id, firstPageId.Value));
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

		var spPage = TfSpaceUIService.GetSpacePage(_spacePage.Id);
		if (spPage == null)
		{
			ToastService.ShowError(LOC("Space node not found"));
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