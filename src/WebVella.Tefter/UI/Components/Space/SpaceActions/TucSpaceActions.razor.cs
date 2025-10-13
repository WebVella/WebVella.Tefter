namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceActions : TfBaseComponent, IAsyncDisposable
{
	private DotNetObjectReference<TucSpaceActions> _objectRef;
	private bool _spaceMenuVisible = false;
	private bool _spaceFinderVisible = false;
	public async ValueTask DisposeAsync()
	{
		try
		{
			await JSRuntime.InvokeAsync<object>("Tefter.removeGlobalSearchKeyListener", ComponentId.ToString());
		}
		catch
		{
			//In rare ocasions the item is disposed after the JSRuntime is no longer avaible
		}
		_objectRef?.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		ComponentId = Guid.NewGuid();
		_objectRef = DotNetObjectReference.Create(this);
		await JSRuntime.InvokeAsync<object>(
			"Tefter.addGlobalSearchKeyListener", _objectRef, ComponentId.ToString(), "OnGlobalSearchHandler");
	}

	private async Task _findSpace()
	{
		if(_spaceFinderVisible) return;
		_spaceFinderVisible = true;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceFinderDialog>(
			TfAuthLayout.GetState().User,
			new DialogParameters()
			{
				PreventDismissOnOverlayClick = false,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false,
			});
		var result = await dialog.Result;
		_spaceFinderVisible = false;
	}

	private void _manageCurrentPage()
	{
		var pageManageUrl = String.Format(TfConstants.SpacePagePageManageUrl, TfAuthLayout.GetState().Space!.Id, TfAuthLayout.GetState().SpacePage!.Id);
		Navigator.NavigateTo(pageManageUrl.GenerateWithLocalAsReturnUrl(Navigator.Uri)!);
	}
	private void _manageCurrentSpace()
	{
		var pageManageUrl = String.Format(TfConstants.SpaceManagePageUrl, TfAuthLayout.GetState().Space!.Id);
		Navigator.NavigateTo(pageManageUrl.GenerateWithLocalAsReturnUrl(Navigator.Uri)!);
	}	
	


	[JSInvokable("OnGlobalSearchHandler")]
	public async Task OnGlobalSearchHandler()
	{
		await _findSpace();
	}	
}