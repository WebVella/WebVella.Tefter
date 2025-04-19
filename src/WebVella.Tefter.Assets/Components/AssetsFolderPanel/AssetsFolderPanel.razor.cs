namespace WebVella.Tefter.Assets.Components;

[LocalizationResource("WebVella.Tefter.Assets.Components.AssetsFolderPanel.AssetsFolderPanel", "WebVella.Tefter.Assets")]
public partial class AssetsFolderPanel : TfFormBaseComponent, IDialogContentComponent<AssetsFolderPanelContext>
{
	[Inject] public IState<TfAppState> TfAppState { get; set; }
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public AssetsFolderPanelContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isLoading = true;

	private AssetsFolder _folder = null;
	private Guid? _skValue = null;
	private Guid _rowId = Guid.Empty;
	private List<Asset> _items = new();
	private Guid? _actionMenuIdOpened = null;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Content.FolderId is not null)
			{
				_folder = AssetsService.GetFolder(Content.FolderId.Value);
				if (_folder is not null && !String.IsNullOrWhiteSpace(_folder.JoinKey) && Content.RowIndex > -1)
				{
					_rowId = (Guid)Content.DataTable.Rows[Content.RowIndex][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
					_skValue = Content.DataTable.Rows[Content.RowIndex].GetJoinKeyValue(_folder.JoinKey);
					if (_skValue is not null)
						_items = AssetsService.GetAssets(_folder.Id, _skValue);
				}
				_isLoading = false;
				await InvokeAsync(StateHasChanged);
			}
			else
			{
				_isLoading = false;
				await InvokeAsync(StateHasChanged);
			}

		}

	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}


	private async Task _addLink()
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelLinkModal>(
		new AssetsFolderPanelLinkModalContext()
		{
			CreatedBy = TfAppState.Value.CurrentUser.Id,
			DataProviderId = TfAppState.Value.SpaceViewData.QueryInfo.DataProviderId,
			FolderId = Content.FolderId.Value,
			RowIds = new List<Guid> { _rowId },
			Id = Guid.Empty,
			Label = null,
			Url = null,
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
			_items.Insert(0, (Asset)result.Data);
		}
	}

	private async Task _addFile()
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelFileModal>(
		new AssetsFolderPanelFileModalContext()
		{
			CreatedBy = TfAppState.Value.CurrentUser.Id,
			DataProviderId = TfAppState.Value.SpaceViewData.QueryInfo.DataProviderId,
			FolderId = Content.FolderId.Value,
			RowIds = new List<Guid> { _rowId },
			Id = Guid.Empty,
			Label = null,
			FileName = null,
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
			_items.Insert(0, (Asset)result.Data);
		}
	}

	private async Task _editAsset(Asset asset)
	{
		DialogResult result = null;

		if (asset.Type == AssetType.File)
		{
			var assetContent = (FileAssetContent)asset.Content;
			var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelFileModal>(
					new AssetsFolderPanelFileModalContext()
					{
						CreatedBy = TfAppState.Value.CurrentUser.Id,
						DataProviderId = TfAppState.Value.SpaceViewData.QueryInfo.DataProviderId,
						FolderId = Content.FolderId.Value,
						RowIds = new List<Guid> { _rowId },
						Id = asset.Id,
						Label = assetContent.Label,
						FileName = assetContent.Filename,
					},
					new DialogParameters()
					{
						PreventDismissOnOverlayClick = true,
						PreventScroll = true,
						Width = TfConstants.DialogWidthLarge,
						TrapFocus = false
					});
			result = await dialog.Result;
		}
		else if (asset.Type == AssetType.Link)
		{
			var assetContent = (LinkAssetContent)asset.Content;
			var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelLinkModal>(
					new AssetsFolderPanelLinkModalContext()
					{
						CreatedBy = TfAppState.Value.CurrentUser.Id,
						DataProviderId = TfAppState.Value.SpaceViewData.QueryInfo.DataProviderId,
						FolderId = Content.FolderId.Value,
						RowIds = new List<Guid> { _rowId },
						Id = asset.Id,
						Label = assetContent.Label,
						Url = assetContent.Url,
						IconUrl = assetContent.IconUrl
					},
					new DialogParameters()
					{
						PreventDismissOnOverlayClick = true,
						PreventScroll = true,
						Width = TfConstants.DialogWidthLarge,
						TrapFocus = false
					});
			result = await dialog.Result;
		}

		if (result is not null && !result.Cancelled && result.Data != null)
		{
			var resultAsset = (Asset)result.Data;
			var index = _items.FindIndex(x => x.Id == resultAsset.Id);
			if (index > -1)
				_items[index] = resultAsset;
		}
	}

	private async Task _deleteAsset(Asset asset)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this file deleted?")))
			return;

		try
		{
			AssetsService.DeleteAsset(asset.Id);
			ToastService.ShowSuccess(LOC("File deleted"));
			int index = _items.FindIndex(x => x.Id == asset.Id);
			if (index > -1) _items.RemoveAt(index);
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

	private async Task _getAsset(Asset asset)
	{
		if (asset.Type == AssetType.File)
		{
			var assetContent = (FileAssetContent)asset.Content;
			await JSRuntime.InvokeVoidAsync("open", assetContent.DownloadUrl, "_blank");
		}
		else if (asset.Type == AssetType.Link)
		{
			var assetContent = (LinkAssetContent)asset.Content;
			await JSRuntime.InvokeVoidAsync("open", assetContent.Url, "_blank");
		}
	}

	private AssetsFolderPanelAssetMeta _getAssetMeta(Asset asset)
	{
		var result = new AssetsFolderPanelAssetMeta();
		if (asset.CreatedOn < asset.ModifiedOn)
		{
			result.Description += $"<span class='updated' title='{asset.ModifiedOn.ToString(TfConstants.DateHourFormat)} by {asset.ModifiedBy.Names}'>updated</span>";
			result.Description += $"<span class='divider'> | </span>";
		}
		result.Description += $" {asset.CreatedOn.ToString(TfConstants.DateHourFormat)}";
		if (asset.CreatedBy is not null)
			result.Description += $" by {asset.CreatedBy.Names}";


		if (asset.Type == AssetType.File)
		{
			var content = (FileAssetContent)asset.Content;
			result.Title = content.Label;
			result.Icon = TfConverters.ConvertFileNameToIcon(content.Filename);
			result.Url = content.DownloadUrl;
		}
		else if (asset.Type == AssetType.Link)
		{
			var content = (LinkAssetContent)asset.Content;
			result.Icon = TfConstants.GetIcon("Link");
			result.Title = content.Label;
			result.Url = content.Url;
			result.FavIcon = content.IconUrl;
		}
		result.Icon = result.Icon.WithColor(Color.Neutral);
		return result;
	}


}

public record AssetsFolderPanelContext
{
	public Guid? FolderId { get; set; }
	public TfDataTable DataTable { get; set; } = null;
	public int RowIndex { get; set; } = -1;
}

public record AssetsFolderPanelAssetMeta
{
	public Icon Icon { get; set; } = TfConstants.GetIcon("Document");
	public string Title { get; set; }
	public string Description { get; set; }
	public string Url { get; set; }
	public string FavIcon { get; set; }
}
