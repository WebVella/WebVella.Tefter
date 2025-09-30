namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewLinkSaveSelector : TfBaseComponent
{
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfDataTable Data { get; set; } = default!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();
	[Parameter] public TfBookmark? ActiveBookmark { get; set; } = null;
	[Parameter] public TfBookmark? ActiveSavedUrl { get; set; } = null;
	private bool _open = false;
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _updateUrl()
	{
		if(ActiveSavedUrl is null) return;
		try
		{
			var submit = ActiveSavedUrl with
			{
				Url = new Uri(Navigator.Uri).PathAndQuery
			};

			TfUIService.UpdateBookmark(submit);

			ToastService.ShowSuccess(LOC("URL updated"));
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

	private async Task _editUrl()
	{
		if(ActiveSavedUrl is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewBookmarkManageDialog>(
						ActiveSavedUrl,
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
		}
	}
	private async Task _saveUrlAs()
	{
		if(ActiveSavedUrl is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewBookmarkManageDialog>(
						ActiveSavedUrl with { Id = Guid.Empty },
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
		}
	}
	private async Task _removeUrl()
	{
		try
		{
			if (ActiveSavedUrl is null)
				return;

			TfUIService.DeleteBookmark(ActiveSavedUrl);

			ToastService.ShowSuccess(LOC("Saved URL removed"));

			await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName,null);
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