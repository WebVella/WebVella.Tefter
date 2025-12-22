using ITfEventBus = WebVella.Tefter.UI.EventsBus.ITfEventBus;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceNavigation : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBus TfEventBus { get; set; } = null!;
	private List<TfMenuItem> _menu = new();
	private string? _dragClass = null;
	CancellationTokenSource? _cancellationTokenSource = null!;
	private readonly int _throttleMs = 500;
	private IAsyncDisposable _spaceUpdatedEventSubscriber = null!;
	private IAsyncDisposable _spacePageEventSubscriber = null!;
	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spaceUpdatedEventSubscriber.DisposeAsync();
		await _spacePageEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		_menu = TfAuthLayout.GetState().Menu;
		_initMenu();
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spacePageEventSubscriber = await TfEventBus.SubscribeAsync<TfSpacePageEventPayload>(
			handler: On_SpacePageEventAsync);
		_spaceUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceUpdatedEventPayload>(
			handler:On_SpaceUpdatedEventAsync);		
		EnableRenderLock();
		_cancellationTokenSource = new();
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri).Menu;
		_initMenu();
		RegenRenderLock();
		StateHasChanged();
	}

	private async Task On_SpaceUpdatedEventAsync(string? key, TfSpaceUpdatedEventPayload? payload)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri).Menu;
		_initMenu();
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);		
	}

	private async Task On_SpacePageEventAsync(string? key, TfSpacePageEventPayload? payload)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri).Menu;
		_initMenu();
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);		
		
	}

	private async Task _onDragOver()
	{
		try
		{
			if (_cancellationTokenSource is not null)
				await _cancellationTokenSource.CancelAsync();
			_dragClass = "tf-drag-container--dragging";
		}
		finally
		{
			RegenRenderLock();
		}
	}

	private async Task _onDragLeave()
	{
		try
		{
			_cancellationTokenSource = new();
			await Task.Delay(_throttleMs);
			if (_cancellationTokenSource.IsCancellationRequested)
			{
				return;
			}

			_dragClass = null;
		}
		catch (OperationCanceledException)
		{
			Console.WriteLine("Method was canceled");
		}
		finally
		{
			RegenRenderLock();
		}
	}

	private async Task _onFileDrop(List<FluentInputFileEventArgs> files)
	{
		if (TfAuthLayout.GetState().NavigationState.SpaceId is null) return;
		if (files.Count == 0)
		{
			ToastService.ShowInfo(LOC("No files found for import"));
			return;
		}

		_dragClass = null;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
		_ = await DialogService.ShowDialogAsync<TucSpacePageImportFromFilesDialog>(
			files,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false,
			});
	}

	private void _initMenu()
	{
		foreach (var menuItem in _menu)
		{
			_initMenuItem(menuItem);
		}
	}

	private void _initMenuItem(TfMenuItem item)
	{
		if (!TfAuthLayout.GetState().User.IsAdmin) return;
		if(item.Actions.Count > 0) return;
		item.Actions.Add(new TfMenuItem()
		{
			Text = LOC("Manage Page"),
			IconCollapsed = TfConstants.GetIcon("Settings"),
			IconExpanded = TfConstants.GetIcon("Settings"),
			OnClick = EventCallback.Factory.Create(this, () => _managePage(item))
		});
		item.Actions.Add(new TfMenuItem()
		{
			Text = LOC("Copy Page"),
			IconCollapsed = TfConstants.GetIcon("Copy"),
			IconExpanded = TfConstants.GetIcon("Copy"),
			OnClick = EventCallback.Factory.Create(this, () => _copyPage(item))
		});
		item.Actions.Add(new TfMenuItem(){IsDivider = true});
		item.Actions.Add(new TfMenuItem()
		{
			Text = LOC("Move up"),
			IconCollapsed = TfConstants.GetIcon("ArrowUp"),
			IconExpanded = TfConstants.GetIcon("ArrowUp"),
			OnClick = EventCallback.Factory.Create(this, () => _movePage(item, true))
		});			
		item.Actions.Add(new TfMenuItem()
		{
			Text = LOC("Move down"),
			IconCollapsed = TfConstants.GetIcon("ArrowDown"),
			IconExpanded = TfConstants.GetIcon("ArrowDown"),
			OnClick = EventCallback.Factory.Create(this, () => _movePage(item, false))
		});			
		item.Actions.Add(new TfMenuItem(){IsDivider = true});
		item.Actions.Add(new TfMenuItem()
		{
			Text = LOC("Delete Page"),
			IconCollapsed = TfConstants.GetIcon("Delete")!.WithColor(Color.Error),
			IconExpanded = TfConstants.GetIcon("Delete")!.WithColor(Color.Error),
			OnClick = EventCallback.Factory.Create(this, async () => await _deletePage(item))
		});		
		foreach (var child in item.Items)
		{
			_initMenuItem(child);
		}
		
	}

	private void _managePage(TfMenuItem item)
	{
		var pageManageUrl = String.Format(TfConstants.SpacePagePageManageUrl, TfAuthLayout.GetState().Space!.Id,
			item.Data!.PageId!);
		Navigator.NavigateTo(pageManageUrl.GenerateWithLocalAndQueryAsReturnUrl(Navigator.Uri)!);		
	}
	private void _copyPage(TfMenuItem item)
	{
		try
		{
			TfService.CopySpacePage(item.Data!.PageId!.Value);
			ToastService.ShowSuccess(LOC("Space page copied!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}	
	private void _movePage(TfMenuItem item, bool isUp)
	{
		try
		{
			TfService.MoveSpacePage(item.Data!.PageId!.Value, isUp);
			ToastService.ShowSuccess(LOC("Space page moved!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}	
	private async Task _deletePage(TfMenuItem item)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this page deleted?")))
			return;		
		try
		{
			ToastService.ShowSuccess(LOC("Space page moved!"));
			var pages = TfService.DeleteSpacePage(item.Data!.PageId!.Value);
			Navigator.NavigateTo(pages.Count > 0
				? String.Format(TfConstants.SpacePagePageUrl, pages[0].SpaceId, pages[0].Id)
				: TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
	private string _getDragStaticOpenClass()
	=> _menu.Count == 0 ? "tf-drag-container--dragging" : "";
}