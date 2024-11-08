using Microsoft.AspNetCore.Components.Web;

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

	private Asset _activeThread = null;
	private Folder _folder = null;
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
				var getFolderResult = AssetsService.GetFolder(Content.FolderId.Value);
				if (getFolderResult.IsSuccess) _folder = getFolderResult.Value;
				else throw new Exception("GetFolder failed");
				if (_folder is not null && !String.IsNullOrWhiteSpace(_folder.SharedKey) && Content.RowIndex > -1)
				{
					_rowId = (Guid)Content.DataTable.Rows[Content.RowIndex][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
					_skValue = Content.DataTable.Rows[Content.RowIndex].GetSharedKeyValue(_folder.SharedKey);
					if (_skValue is not null)
					{
						var getAssetsResult = AssetsService.GetAssets(_folder.Id, _skValue);
						if (getAssetsResult.IsSuccess) _items = getAssetsResult.Value;
						else throw new Exception("GetAssets failed");
					}
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
			var result = AssetsService.DeleteAsset(asset.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("File deleted"));
				int index = _items.FindIndex(x => x.Id == asset.Id);
				if (index > -1) _items.RemoveAt(index);
			}
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

	private (Icon, string, string) _getAssetMeta(Asset asset)
	{
		Icon icon = TfConstants.GetIcon("Document");
		string title = null;
		string description = null;
		description = $"{LOC("created on")}: {asset.CreatedOn.ToString(TfConstants.DateFormat)}";
		if (asset.Type == AssetType.File)
		{
			var content = (FileAssetContent)asset.Content;
			title = content.Label;
			icon = TfConverters.ConvertFileNameToIcon(content.Filename);
		}
		else if (asset.Type == AssetType.Link)
		{
			icon = TfConstants.GetIcon("Link");
			var content = (LinkAssetContent)asset.Content;
			title = content.Label;
		}
		icon = icon.WithColor(Color.Neutral);
		return (icon, title, description);
	}

	private void _itemClick()
	{
		ToastService.ShowSuccess("_itemClick");
	}

	private void _actionClick()
	{
		ToastService.ShowSuccess("_actionClick");
	}

}

public record AssetsFolderPanelContext
{
	public Guid? FolderId { get; set; }
	public TfDataTable DataTable { get; set; } = null;
	public int RowIndex { get; set; } = -1;

}
