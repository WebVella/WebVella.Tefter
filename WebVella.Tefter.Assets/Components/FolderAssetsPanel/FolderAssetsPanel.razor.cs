namespace WebVella.Tefter.Assets.Components;

[LocalizationResource("WebVella.Tefter.Assets.Components.FolderAssetsPanel.FolderAssetsPanel", "WebVella.Tefter.Assets")]
public partial class FolderAssetsPanel : TfFormBaseComponent, IDialogContentComponent<FolderAssetsPanelContext>
{
	[Inject] public IState<TfAppState> TfAppState { get; set; }
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public FolderAssetsPanelContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isLoading = true;

	private Asset _activeThread = null;
	private Folder _folder = null;
	private Guid? _skValue = null;
	private Guid _rowId = Guid.Empty;
	private List<Asset> _items = new();

	private Guid? _assetEditedId = null;
	private Guid? _assetIdUpdateSaving = null;
	private bool _assetBroadcastVisible = false;
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

	private async Task _addAsset()
	{
		Console.WriteLine("_sendMessage");
		//if (_channelEditorSending) return;
		//_channelEditorSending = true;
		//await InvokeAsync(StateHasChanged);
		//try
		//{
		//	var submit = new CreateTalkThread
		//	{
		//		ChannelId = _folder.Id,
		//		Content = _channelEditorContent,
		//		Type = TalkThreadType.Comment,
		//		UserId = TfUserState.Value.CurrentUser.Id,
		//		DataProviderId = Content.DataTable.QueryInfo.DataProviderId,
		//		RowIds = new List<Guid> { _rowId }
		//	};
		//	var result = AssetsService.CreateThread(submit);
		//	ProcessServiceResponse(result);
		//	if (result.IsSuccess)
		//	{
		//		ToastService.ShowSuccess(LOC("Message is sent"));
		//		_channelEditorContent = null;
		//		var getThreadResult = AssetsService.GetThread(result.Value);
		//		if (getThreadResult.IsFailed) throw new Exception("GetThreads failed");
		//		_items.Insert(0, getThreadResult.Value);

		//	}
		//}
		//catch (Exception ex)
		//{
		//	ProcessException(ex);
		//}
		//finally
		//{
		//	_channelEditorSending = false;
		//	await InvokeAsync(StateHasChanged);
		//}
	}

	private async Task _editAsset(Asset asset)
	{
		if (_assetEditedId is not null)
		{
			if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("You will loose any unsaved changes on your previous edit. Do you want to continue?")))
				return;
		}

		if (_assetEditedId == asset.Id) _assetEditedId = null;
		else _assetEditedId = asset.Id;
	}

	private async Task _deleteAsset(Asset asset)
	{

		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this thread deleted?")))
			return;

		try
		{
			var result = AssetsService.DeleteAsset(asset.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message deleted"));
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

	private async Task _saveAsset(Asset asset, string content)
	{
		if (_assetIdUpdateSaving is not null) return;
		_assetIdUpdateSaving = asset.Id;
		await InvokeAsync(StateHasChanged);
		try
		{
			var assetContent = new AssetContentBase();
			var result = AssetsService.UpdateAssetContent(asset.Id, assetContent,TfAppState.Value.CurrentUser.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Asset saved"));
				int threadsIndex = _items.FindIndex(x => x.Id == asset.Id);
				//TODO
				//if (threadsIndex > -1) _items[threadsIndex].Content = content;

				_assetEditedId = null;
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_assetIdUpdateSaving = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private Task _cancelSaveMessage()
	{

		_assetEditedId = null;
		return Task.CompletedTask;
	}

	private Task _showThreadBradcastDetails()
	{
		_assetBroadcastVisible = !_assetBroadcastVisible;
		return Task.CompletedTask;
	}
}

public record FolderAssetsPanelContext
{
	public Guid? FolderId { get; set; }
	public TfDataTable DataTable { get; set; } = null;
	public int RowIndex { get; set; } = -1;

}
