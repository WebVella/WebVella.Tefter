namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceActions : TfBaseComponent, IAsyncDisposable
{
	private DotNetObjectReference<TucSpaceActions> _objectRef;
	private bool _spaceFinderVisible = false;
	public async ValueTask DisposeAsync()
	{
		try
		{
			await JSRuntime.InvokeAsync<object>("Tefter.removeGlobalSearchKeyListener", ComponentId.ToString());
		}
		catch
		{
			//In rare occasions the item is disposed after the JSRuntime is no longer available
		}

		_objectRef?.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		ComponentId = Guid.NewGuid();
		_objectRef = DotNetObjectReference.Create(this);
		await JSRuntime.InvokeAsync<object>(
			"Tefter.addGlobalSearchKeyListener", _objectRef, ComponentId.ToString(), "OnGlobalSearchHandler");

	}

	private async Task _findSpace()
	{
		if (_spaceFinderVisible) return;
		_spaceFinderVisible = true;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceFinderDialog>(
			TfAuthLayout.GetState().User,
			new()
			{
				PreventDismissOnOverlayClick = false,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false,
			});
		_ = await dialog.Result;
		_spaceFinderVisible = false;
	}

	private bool _isHomeSpace()
		=> TfAuthLayout.GetState().NavigationState.HasNode(RouteDataNode.Home, 0);


	[JSInvokable("OnGlobalSearchHandler")]
	public async Task OnGlobalSearchHandler()
	{
		await _findSpace();
	}


}