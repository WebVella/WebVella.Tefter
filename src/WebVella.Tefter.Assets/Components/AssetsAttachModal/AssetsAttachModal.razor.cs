namespace WebVella.Tefter.Talk.Components;
public partial class AssetsAttachModal : TfFormBaseComponent, IDialogContentComponent<AssetsAttachModalContext>
{
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public AssetsAttachModalContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _title = string.Empty;
	private string _error = string.Empty;
	private bool _isLoading = true;
	private bool _isSubmitting = false;
	private List<AssetsFolder> _folders = new();
	private AssetsFolder _selectedFolder = null;
	private string _content = null;
	private List<object> _submits = new();
	private long _countChange = 0;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Content is null
			|| Content.DataProviderId == Guid.Empty
			|| Content.SelectedRowIds is null || Content.SelectedRowIds.Count == 0) return;

			var providerIdentities = AssetsService.GetDataProviderIdentities(Content.DataProviderId);

			var allFolders = AssetsService.GetFolders();

			//Select only channels that are compatible with this DataProvider
			foreach (var folder in allFolders)
			{
				if (String.IsNullOrWhiteSpace(folder.DataIdentity)) continue;
				if (!providerIdentities.Any(x => x.DataIdentity == folder.DataIdentity)) continue;
				_folders.Add(folder);
			}

			if (_folders.Count == 1)
			{
				await _selectChannelHandler(_folders[0]);
			}
			else if (_folders.Count > 1)
			{
				_title = LOC("Select a folder");
			}


			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _addLink()
	{
		if (_selectedFolder is null) return;
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelLinkModal>(
		new AssetsFolderPanelLinkModalContext()
		{
			UserId = Content.CurrentUser.Id,
			FolderId = _selectedFolder.Id,
			Id = Guid.Empty,
			Label = null,
			Url = null,
			DataProviderId = Content.DataProviderId,
			RowIds = Content.SelectedRowIds,
			IsAddOnly = true
		},
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false,
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var submit = (CreateLinkAssetWithRowIdModel)result.Data;
			_submits.Add(submit);
			_countChange++;
		}
	}

	private async Task _addFile()
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelFileModal>(
		new AssetsFolderPanelFileModalContext()
		{
			UserId = Content.CurrentUser.Id,
			FolderId = _selectedFolder.Id,
			Id = Guid.Empty,
			Label = null,
			DataProviderId = Content.DataProviderId,
			RowIds = Content.SelectedRowIds,
			FileName = null,
			IsAddOnly = true
		},
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
			var submit = (CreateFileAssetWithRowIdModel)result.Data;
			_submits.Add(submit);
			_countChange++;
		}
	}
	private void _removeSubmit(object submit)
	{
		_submits.Remove(submit);
		_countChange--;
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		_isSubmitting = true;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		try
		{
			AssetsService.CreateMultipleAssets(_submits);
			ToastService.ShowSuccess(LOC("Assets were attached successfully"));
			_content = null;
			_submits = new();

			if (_countChange > 0 
				&& !String.IsNullOrWhiteSpace(_selectedFolder.DataIdentity)
				&& !String.IsNullOrWhiteSpace(_selectedFolder.CountSharedColumnName))
			{
				var columnName = $"{_selectedFolder.DataIdentity}.{_selectedFolder.CountSharedColumnName}";
				foreach (var rowId in Content.SelectedRowIds)
				{
					Content.CountChange[rowId] = new();
					Content.CountChange[rowId][columnName] = _countChange;
				}
			}
			await _cancel();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _selectChannelHandler(AssetsFolder folder)
	{
		_selectedFolder = folder;
		_title = $"#{_selectedFolder.Name}";
	}
}

public record AssetsAttachModalContext
{
	public TfUser CurrentUser { get; set; }
	public Guid DataProviderId { get; set; }
	public List<Guid> SelectedRowIds { get; set; } = new();
	public Dictionary<Guid, Dictionary<string, long>> CountChange { get; set; } = new();

}