namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageDetails : TfBaseComponent, IAsyncDisposable
{
	private bool _isRemoving = false;
	private TfSpacePage? _spacePage = null;
	private TfSpace? _space = null;
	private TfNavigationState? _navState = null;
	private IAsyncDisposable _spacePageUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spacePageUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spacePageUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpacePageUpdatedEventPayload>(
			handler: On_SpacePageUpdatedEventAsync);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpacePageUpdatedEventAsync(string? key, TfSpacePageUpdatedEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
			if (_navState.SpacePageId.HasValue && _spacePage?.Id != _navState.SpacePageId)
			{
				_spacePage = TfService.GetSpacePage(_navState.SpacePageId.Value);
				_space = null;
				if (_spacePage is not null)
					_space = TfService.GetSpace(_spacePage.SpaceId);
			}
		}
		finally
		{
			UriInitialized = _navState?.Uri ?? String.Empty;
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
				CurrentUser = TfAuthLayout.GetState().User,
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
			TfService.DeleteSpacePage(_spacePage.Id);
			ToastService.ShowSuccess(LOC("Space page deleted!"));
			var spacePages = TfService.GetSpacePages(_spacePage.SpaceId);
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
				Navigator.NavigateTo(string.Format(TfConstants.SpacePagePageUrl, _spacePage.SpaceId,
					firstPageId.Value));
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

		var spPage = TfService.GetSpacePage(_spacePage.Id);
		if (spPage == null)
		{
			ToastService.ShowError(LOC("Space page not found"));
			return;
		}

		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			spPage,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
		{
			ToastService.ShowSuccess(LOC("Space page successfully saved!"));
		}
	}
}