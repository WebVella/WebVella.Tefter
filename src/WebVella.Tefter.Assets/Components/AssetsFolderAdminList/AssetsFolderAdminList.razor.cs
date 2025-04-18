﻿using Microsoft.JSInterop;

namespace WebVella.Tefter.Assets.Components;
[LocalizationResource("WebVella.Tefter.Assets.Components.AssetsFolderAdminList.AssetsFolderAdminList", "WebVella.Tefter.Assets")]
public partial class AssetsFolderAdminList : TfBaseComponent
{
	[Inject] public IAssetsService AssetsService { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }

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
		if (!result.Cancelled && result.Data != null)
		{
			List<AssetsFolder> state = new();
			if (TfAuxDataState.Value.Data.ContainsKey(AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY))
				state = (List<AssetsFolder>)TfAuxDataState.Value.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY];
			state.Add((AssetsFolder)result.Data);
			TfAuxDataState.Value.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = state;
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: this,
				state: TfAuxDataState.Value
			));
			ToastService.ShowSuccess(LOC("Folder successfully created!"));

		}
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
		if (!result.Cancelled && result.Data != null)
		{
			var item = (AssetsFolder)result.Data;
			List<AssetsFolder> state = new();
			if (TfAuxDataState.Value.Data.ContainsKey(AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY))
				state = (List<AssetsFolder>)TfAuxDataState.Value.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY];
			var itemIndex = state.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				state[itemIndex] = item;
			}
			TfAuxDataState.Value.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = state;
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: this,
				state: TfAuxDataState.Value
			));
			ToastService.ShowSuccess(LOC("Folder successfully updated!"));

		}
	}
	private async Task _deleteFolderHandler(AssetsFolder folder)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this folder deleted? This will delete all the files in it!")))
			return;
		try
		{
			AssetsService.DeleteFolder(folder.Id);

			List<AssetsFolder> state = new();
			if (TfAuxDataState.Value.Data.ContainsKey(AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY))
				state = (List<AssetsFolder>)TfAuxDataState.Value.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY];
			var itemIndex = state.FindIndex(x => x.Id == folder.Id);
			if (itemIndex > -1)
			{
				state.RemoveAt(itemIndex);
			}
			TfAuxDataState.Value.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = state;
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: this,
				state: TfAuxDataState.Value
			));
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