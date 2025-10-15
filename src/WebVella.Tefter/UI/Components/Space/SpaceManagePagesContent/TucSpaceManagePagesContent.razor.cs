namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceManagePagesContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private TfSpace? _space = null;
	private List<TfSpacePage> _spacePages = new();
	private TfNavigationState _navState = null!;
	public bool _submitting = false;

	public void Dispose()
	{
		TfEventProvider.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.SpacePageCreatedEvent += On_SpacePageChanged;
		TfEventProvider.SpacePageUpdatedEvent += On_SpacePageChanged;
		TfEventProvider.SpacePageDeletedEvent += On_SpacePageChanged;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_SpacePageChanged(object args)
	{
		await _init(TfAuthLayout.GetState().NavigationState);
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
			if (_space is null)
				_space = TfService.GetSpace(_navState.SpaceId.Value);
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
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			new TfSpacePage() { SpaceId = _space.Id },
			new ()
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
			TfService.DeleteSpacePage(node);
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
			TfService.MoveSpacePage(args.Item1, args.Item2);

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
			TfService.CopySpacePage(nodeId);

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
			new ()
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