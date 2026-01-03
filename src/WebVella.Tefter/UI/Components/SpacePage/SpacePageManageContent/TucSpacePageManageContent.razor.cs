namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageManageContent : TfBaseComponent, IAsyncDisposable
{
	private bool _isDeleting = false;
	private TfSpace _space = null!;
	private TfSpacePage _spacePage = null!;
	private List<TfTag> _pageTags = new();
	private TfSpacePageAddonMeta? _component = null;
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
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpacePageUpdatedEventAsync(string? key, TfSpacePageUpdatedEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);


	private async Task _init(TfNavigationState navState)
	{
		try
		{
			if (navState.SpaceId is null)
				throw new Exception("Space Id not found in URL");
			_space = TfService.GetSpace(navState.SpaceId.Value) ?? throw new Exception("Space not found");
			if (navState.SpacePageId is null)
				throw new Exception("Page Id not found in URL");
			_spacePage = TfService.GetSpacePage(navState.SpacePageId.Value) ?? throw new Exception("Space page not found");
			var pageMeta = TfMetaService.GetSpacePagesComponentsMeta();
			_component = pageMeta.FirstOrDefault(x => x.Instance.AddonId == _spacePage.ComponentId);

			_pageTags = TfService.GetSpacePageTags(_spacePage.Id);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editPage()
	{
		_ = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			_spacePage,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
	}

	private async Task _deletePage()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this page deleted?")))
			return;

		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			TfService.DeleteSpacePage(_spacePage.Id);
			var pages = TfService.GetSpacePages(_space.Id);
			ToastService.ShowSuccess(LOC("Page was successfully deleted"));
			Navigator.NavigateTo(pages.Count > 0
				? String.Format(TfConstants.SpacePagePageUrl, _space.Id, pages[0].Id)
				: TfConstants.HomePageUrl);
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