namespace WebVella.Tefter.Assets.Components;
public partial class AssetsFolderAdminList : TfBaseComponent
{
	[Inject] public IAssetsService AssetsService { get; set; }

	private List<AssetsFolder> _folders = new();
	private TfNavigationState _navState = new();

	public void Dispose()
	{
		AssetsService.FolderCreated -= On_FolderChanged;
		AssetsService.FolderUpdated -= On_FolderChanged;
		AssetsService.FolderDeleted -= On_FolderChanged;
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		AssetsService.FolderCreated += On_FolderChanged;
		AssetsService.FolderUpdated += On_FolderChanged;
		AssetsService.FolderDeleted += On_FolderChanged;
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}
	private async Task On_FolderChanged(AssetsFolder args)
	{
		await InvokeAsync(async () => { await _init(TfState.NavigationState); });
	}
	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(args);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
			_folders = AssetsService.GetFolders();
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _createFolderHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderManageDialog>(
		new AssetsFolder(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null){}
	}

	private async Task _editFolderHandler(AssetsFolder folder)
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderManageDialog>(
		folder,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null){}
	}
	private async Task _deleteFolderHandler(AssetsFolder folder)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this folder deleted? This will delete all the files in it!")))
			return;
		try
		{
			AssetsService.DeleteFolder(folder.Id);
			ToastService.ShowSuccess(LOC("Folder successfully deleted!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}

}