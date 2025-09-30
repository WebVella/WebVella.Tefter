namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceManagePagesContent : TfBaseComponent, IDisposable
{
	private TfSpace _space = default!;
	private List<TfSpacePage> _spacePages = default!;
	private TfNavigationState _navState = default!;
	public bool _submitting = false;
	public void Dispose()
	{
		TfUIService.SpaceUpdated -= On_SpaceUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.SpaceUpdated += On_SpaceUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceUpdated(object? caller, TfSpace args)
	{
		await _init(space: args);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfSpace? space = null)
	{
		if (navState == null)
			_navState = await TfUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;

		try
		{
			if (space is not null)
			{
				_space = space;
			}
			else
			{
				if (_navState.SpaceId is null) return;
				_space = TfUIService.GetSpace(_navState.SpaceId.Value);
				if (_space is null) return;
			}
			_spacePages = TfUIService.GetSpacePages(_space.Id);

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
		new TfSpacePage() { SpaceId = _space.Id},
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

	private async Task _removePage(TfSpacePage node)
	{
		if (_submitting) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this page deleted?")))
			return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			TfUIService.DeleteSpacePage(node);
			ToastService.ShowSuccess(LOC("Space page deleted!"));
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
			TfUIService.MoveSpacePage(args.Item1, args.Item2);

			ToastService.ShowSuccess(LOC("Space page updated!"));
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
			TfUIService.CopySpacePage(nodeId);

			ToastService.ShowSuccess(LOC("Space page updated!"));
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
}