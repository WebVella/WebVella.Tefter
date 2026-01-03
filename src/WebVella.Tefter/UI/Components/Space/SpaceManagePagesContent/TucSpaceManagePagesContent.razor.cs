namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceManagePagesContent : TfBaseComponent, IAsyncDisposable
{
	private TfSpace? _space = null;
	private List<TfSpacePage> _spacePages = new();
	private TfNavigationState _navState = null!;
	private bool _submitting = false;
	private IAsyncDisposable _spacePageEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spacePageEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spacePageEventSubscriber = await TfEventBus.SubscribeAsync<TfSpacePageEventPayload>(
			handler: On_SpacePageEventAsync,
			matchKey: (_) => true);
	}

	private async Task On_SpacePageEventAsync(string? key, TfSpacePageEventPayload? payload)
	{
		if(payload is null) return;
		if(payload.SpacePage.SpaceId != _space?.Id) return;
		if(key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
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
		if (_navState.SpaceId is null)
			throw new Exception("No space Id defined in the URL");

		try
		{
			_space ??= TfService.GetSpace(_navState.SpaceId.Value);
			if (_space is null) throw new Exception("Space not found");
			_spacePages = TfService.GetSpacePages(_space.Id);
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _addPage()
	{
		if(_space is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			new TfSpacePage() { SpaceId = _space.Id },
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }){}
	}

	private async Task _removePage(TfSpacePage node)
	{
		if (_submitting) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this page deleted?")))
			return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			TfService.DeleteSpacePage(node.Id);
			ToastService.ShowSuccess(LOC("Space page deleted!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageDeletedEventPayload(node));					
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _movePage(Tuple<TfSpacePage, bool> args)
	{
		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			TfService.MoveSpacePage(args.Item1.Id, args.Item2);
			ToastService.ShowSuccess(LOC("Space page updated!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageUpdatedEventPayload(args.Item1));				
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _copyPage(Guid nodeId)
	{
		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			var (pageId,pages) = TfService.CopySpacePage(nodeId);
			var page = pages.Single(x=> x.Id == pageId);
			ToastService.ShowSuccess(LOC("Space page updated!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageCreatedEventPayload(page));					
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editPage(Guid nodeId)
	{
		var node = _spacePages.GetSpaceNodeById(nodeId);
		if (node == null)
		{
			ToastService.ShowError(LOC("Space page not found"));
			return;
		}

		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			node,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }){}
	}
}