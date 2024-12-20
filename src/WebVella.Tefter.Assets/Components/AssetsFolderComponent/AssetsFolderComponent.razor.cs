

using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.Assets.Components;

[LocalizationResource("WebVella.Tefter.Assets.Components.AssetsFolderComponent.AssetsFolderComponent", "WebVella.Tefter.Assets")]
public partial class AssetsFolderComponent : TfBaseComponent
{
	[Inject] public IState<TfAppState> TfAppState { get; set; }
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public Guid? FolderId { get; set; }
	[Parameter] public Guid? SharedKeyValue { get; set; }
	[Parameter] public TucUser CurrentUser { get; set; }
	[Parameter] public string Style { get; set; } = "";
	[Parameter] public RenderFragment HeaderActions { get; set; }

	private string _error = string.Empty;
	private bool _isLoading = true;

	private TfEditor _folderEditor = null;
	private string _folderEditorContent = null;
	private bool _folderEditorSending = false;

	private TfEditor _assetEditor = null;
	private string _assetEditorContent = null;
	private bool _assetEditorSending = false;

	private Asset _activeAsset = null;
	private AssetsFolder _folder = null;

	private Guid _rowId = Guid.Empty;
	private List<Asset> _items = new();
	private List<Asset> _allItems = new();
	private Guid? _actionMenuIdOpened = null;
	private string _search = null;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (FolderId is not null)
			{
				var getFolderResult = AssetsService.GetFolder(FolderId.Value);
				if (getFolderResult.IsSuccess) _folder = getFolderResult.Value;
				else throw new Exception("GetFolder failed");
				if (_folder is not null && SharedKeyValue is not null)
				{
					var getAssetsResult = AssetsService.GetAssets(_folder.Id, SharedKeyValue.Value);
					if (getAssetsResult.IsSuccess) _allItems = getAssetsResult.Value;
					else throw new Exception("GetAssets failed");
				}
				_isLoading = false;
				_search = TfAppState.Value.Route.Search;
				_generateItems();
				await InvokeAsync(StateHasChanged);
				ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
			}
			else
			{
				_isLoading = false;
				await InvokeAsync(StateHasChanged);
			}

		}

	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_search = TfAppState.Value.Route.Search;
			_generateItems();
			await InvokeAsync(StateHasChanged);
		});
	}
	private async Task _searchValueChanged(string search)
	{
		_search = search?.Trim();
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.SearchQueryName] = _search;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private void _generateItems()
	{
		_items.Clear();
		if (String.IsNullOrWhiteSpace(_search))
		{
			_items = _allItems.ToList();
			return;
		}
		var searchLowered = _search?.Trim().ToLowerInvariant();
		foreach (var item in _allItems)
		{
			if (item.Type == AssetType.File)
			{
				var content = (FileAssetContent)item.Content;
				if (content.Label.ToLower().Contains(searchLowered)
					|| content.Filename.ToLower().Contains(searchLowered))
					_items.Add(item);
			}
			else if (item.Type == AssetType.Link)
			{
				var content = (LinkAssetContent)item.Content;
				if (content.Label.ToLower().Contains(searchLowered)
					|| content.Url.ToLower().Contains(searchLowered))
					_items.Add(item);
			}
		}

	}

	private async Task _addLink()
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelLinkModal>(
		new AssetsFolderPanelLinkModalContext()
		{
			CreatedBy = TfAppState.Value.CurrentUser.Id,
			FolderId = _folder.Id,
			SKValueIds = new List<Guid> { TfAppState.Value.SpaceNode.Id },
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
			FolderId = _folder.Id,
			SKValueIds = new List<Guid> { TfAppState.Value.SpaceNode.Id },
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
						SKValueIds = new List<Guid> { TfAppState.Value.SpaceNode.Id },
						FolderId = _folder.Id,
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
						SKValueIds = new List<Guid> { TfAppState.Value.SpaceNode.Id },
						FolderId = _folder.Id,
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

	private AssetsFolderComponentAssetMeta _getAssetMeta(Asset asset)
	{
		var result = new AssetsFolderComponentAssetMeta();

		if (asset.Type == AssetType.File)
		{
			var content = (FileAssetContent)asset.Content;
			result.Title = content.Label;
			result.Icon = TfConverters.ConvertFileNameToIcon(content.Filename);
			result.Url = content.DownloadUrl;
			if (content.Label != content.Filename)
			{
				result.Description += $"<span class='source'>{content.Filename}</span>";
				result.Description += $"<span class='divider'> | </span>";
			}
		}
		else if (asset.Type == AssetType.Link)
		{
			var content = (LinkAssetContent)asset.Content;
			result.Icon = TfConstants.GetIcon("Link");
			result.Title = content.Label;
			result.Url = content.Url;
			result.FavIcon = content.IconUrl;
			var domain = UrlUtility.GetDomainFromUrl(result.Url);
			if (!String.IsNullOrWhiteSpace(domain))
			{
				result.Description += $"<span class='source'>{domain}</span>";
				result.Description += $"<span class='divider'> | </span>";
			}
		}
		result.Icon = result.Icon.WithColor(Color.Neutral);
		if (asset.CreatedOn < asset.ModifiedOn)
		{
			result.Description += $"<span class='updated' title='{asset.ModifiedOn.ToString(TfConstants.DateHourFormat)} by {asset.ModifiedBy.Names}'>updated</span>";
			result.Description += $"<span class='divider'> | </span>";
		}
		result.Description += $" {asset.CreatedOn.ToString(TfConstants.DateHourFormat)}";
		if (asset.CreatedBy is not null)
			result.Description += $" by {asset.CreatedBy.Names}";
		return result;
	}
}

public record AssetsFolderComponentAssetMeta
{
	public Icon Icon { get; set; } = TfConstants.GetIcon("Document");
	public string Title { get; set; }
	public string Description { get; set; }
	public string Url { get; set; }
	public string FavIcon { get; set; }
}
