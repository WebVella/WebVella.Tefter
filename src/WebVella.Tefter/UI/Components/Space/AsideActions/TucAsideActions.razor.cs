namespace WebVella.Tefter.UI.Components;

public partial class TucAsideActions : TfBaseComponent
{
	private bool _spaceMenuVisible = false;
	private bool _spaceFinderVisible = false;

	private async Task _addPage()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
			new TfSpacePage() { SpaceId = TfAuthLayout.GetState().Space!.Id },
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var page = (TfSpacePage)result.Data;
			Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl,page.SpaceId,page.Id));
		}
	}

	private async Task _copyPage()
	{
		var state = TfAuthLayout.GetState();
		if (state.SpacePage is null) return;
		try
		{
			var(pageId,pages) = TfService.CopySpacePage(state.SpacePage.Id);
			ToastService.ShowSuccess(LOC("Space page updated!"));
			var page = pages.Single(x=> x.Id == pageId);
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageCreatedEventPayload(page));				
			Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl,page.SpaceId,page.Id));
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

	private async Task _removePage()
	{
		var state = TfAuthLayout.GetState();
		if (state.SpacePage is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this page deleted?")))
			return;

		try
		{
			var pages = TfService.DeleteSpacePage(state.SpacePage.Id);
			ToastService.ShowSuccess(LOC("Space page deleted!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageDeletedEventPayload(state.SpacePage));				
			if (pages.Count > 0)
			{
				Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl,pages[0].SpaceId,pages[0].Id));
			}
			else
			{
				Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl,state.NavigationState.SpaceId));
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

	private void _manageCurrentPage()
	{
		var pageManageUrl = String.Format(TfConstants.SpacePagePageManageUrl, TfAuthLayout.GetState().Space!.Id,
			TfAuthLayout.GetState().SpacePage!.Id);
		Navigator.NavigateTo(pageManageUrl.GenerateWithLocalAndQueryAsReturnUrl(Navigator.Uri)!);
	}

	private void _manageCurrentSpace()
	{
		var pageManageUrl = String.Format(TfConstants.SpaceManagePageUrl, TfAuthLayout.GetState().Space!.Id);
		Navigator.NavigateTo(pageManageUrl.GenerateWithLocalAndQueryAsReturnUrl(Navigator.Uri)!);
	}

	
	private async Task _deleteSpace()
	{
		var state = TfAuthLayout.GetState();
		if (state.Space is null) return;		
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;
		try
		{
			TfService.DeleteSpace(state.Space.Id);
			ToastService.ShowSuccess(LOC("Space deleted"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpaceDeletedEventPayload(state.Space));				
			Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}	
	

}