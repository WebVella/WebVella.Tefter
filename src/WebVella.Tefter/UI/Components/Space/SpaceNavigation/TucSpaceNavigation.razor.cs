using Microsoft.FluentUI.AspNetCore.Components.Utilities;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceNavigation : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private List<TfMenuItem> _menu = new();
	private string? _dragClass = null;
	CancellationTokenSource? _cancellationTokenSource = null!;
	private int _throttleMS = 500;

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.Dispose();
	}

	protected override void OnInitialized()
	{
		_menu = TfAuthLayout.GetState().Menu;
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpaceUpdatedEvent += On_SpaceOrPageUpdated;
		TfEventProvider.SpacePageCreatedEvent += On_SpaceOrPageUpdated;
		TfEventProvider.SpacePageUpdatedEvent += On_SpaceOrPageUpdated;
		TfEventProvider.SpacePageDeletedEvent += On_SpaceOrPageUpdated;
		EnableRenderLock();
		_cancellationTokenSource = new();
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri, null).Menu;
		RegenRenderLock();
		StateHasChanged();
	}

	private async Task On_SpaceOrPageUpdated(object args)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri, null).Menu;
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
			await Task.Delay(_throttleMS);
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
		if (files.Count == 0) return;
		_dragClass = null;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);		
		var dialog = await DialogService.ShowDialogAsync<TucPageImportFromFilesDialog>(
			files,
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
			TfService.DeleteFiles(files);	
		}

	}
}