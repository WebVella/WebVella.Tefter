

using WebVella.Tefter.UIServices;

namespace WebVella.Tefter.Assets.Components;
public partial class AssetsFolderComponent : TfBaseComponent, IDisposable
{
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public AssetsFolder? Folder { get; set; } = null;
	[Parameter] public string DataIdentityValue { get; set; } = null;
	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;
	[Parameter] public string Style { get; set; } = "";
	[Parameter] public RenderFragment HeaderActions { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isLoading = true;

	private Guid _rowId = Guid.Empty;
	private List<Asset> _items = new();
	private Guid? _actionMenuIdOpened = null;
	private string _search = null;
	private string _dataIdentityValue = null;
	private Guid? _folderId = null;
	private Guid? _pageId = null;
	private FluentSearch? _refSearch = null;
	public void Dispose()
	{
		AssetsService.AssetCreated -= On_AssetChanged;
		AssetsService.AssetUpdated -= On_AssetChanged;
		AssetsService.AssetDeleted -= On_AssetChanged;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		_isLoading = false;
		AssetsService.AssetCreated += On_AssetChanged;
		AssetsService.AssetUpdated += On_AssetChanged;
		AssetsService.AssetDeleted += On_AssetChanged;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender && _refSearch != null)
		{
			if (Context.SpacePage?.Id != _pageId)
			{
				_refSearch.FocusAsync();
				_pageId = Context.SpacePage.Id;
			}
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (_folderId == Folder?.Id && _dataIdentityValue == DataIdentityValue)
			return;
		await _init();
		await InvokeAsync(StateHasChanged);

	}

	private async void On_AssetChanged(object? caller, Asset args)
	{
		await _init();
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}
	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = TfAuthLayout.NavigationState;
		try
		{
			_items = new();
			if (Folder is null) return;
			_search = Navigator.GetStringFromQuery(TfConstants.SearchQueryName, null);
			_items = AssetsService.GetAssets(
				folderId: Folder.Id,
				dataIdentityValue: DataIdentityValue,
				search: _search);
			_dataIdentityValue = DataIdentityValue;
			_folderId = Folder.Id;
			if (_pageId is not null && Context.SpacePage?.Id != _pageId)
			{
				_refSearch?.FocusAsync();
				_pageId = Context.SpacePage?.Id;
			}
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _searchValueChanged(string search)
	{
		_search = search?.Trim();
		var queryDict = new Dictionary<string, object>{
			{TfConstants.SearchQueryName,_search}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _addLink()
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelLinkModal>(
			new AssetsFolderPanelLinkModalContext()
			{
				UserId = Context.CurrentUser.Id,
				FolderId = Folder.Id,
				Id = Guid.Empty,
				Label = null,
				Url = null,
				DataIdentities = new List<string> { _dataIdentityValue },
			},
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

	private async Task _addFile()
	{
		var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelFileModal>(
			new AssetsFolderPanelFileModalContext()
			{
				UserId = Context.CurrentUser.Id,
				FolderId = Folder.Id,
				Id = Guid.Empty,
				Label = null,
				FileName = null,
				DataIdentityValues = new List<string> { _dataIdentityValue },
			},
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

	private async Task _editAsset(Asset asset)
	{
		DialogResult result = null;

		if (asset.Type == AssetType.File)
		{
			var assetContent = (FileAssetContent)asset.Content;
			var dialog = await DialogService.ShowDialogAsync<AssetsFolderPanelFileModal>(
					new AssetsFolderPanelFileModalContext()
					{
						UserId = Context.CurrentUser.Id,
						FolderId = Folder.Id,
						Id = asset.Id,
						Label = assetContent.Label,
						FileName = assetContent.Filename,
						DataIdentityValues = new List<string> { _dataIdentityValue },
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
						UserId = Context.CurrentUser.Id,
						FolderId = Folder.Id,
						RowIds = new List<Guid> { _rowId },
						Id = asset.Id,
						Label = assetContent.Label,
						Url = assetContent.Url,
						IconUrl = assetContent.IconUrl,
						DataIdentities = new List<string> { _dataIdentityValue },
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

		if (result is not null && !result.Cancelled && result.Data != null){}
	}

	private async Task _deleteAsset(Asset asset)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this file deleted?")))
			return;

		try
		{
			AssetsService.DeleteAsset(asset.Id);
			ToastService.ShowSuccess(LOC("File deleted"));
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
			result.Description += $"<span class='type file'>{LOC("file")}</span>";
			result.Description += $"<span class='divider'> | </span>";
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
			result.Description += $"<span class='type link'>{LOC("link")}</span>";
			result.Description += $"<span class='divider'> | </span>";
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
