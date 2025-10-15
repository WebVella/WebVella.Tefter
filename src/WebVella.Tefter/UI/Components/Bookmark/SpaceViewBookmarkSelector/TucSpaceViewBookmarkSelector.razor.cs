namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewBookmarkSelector : TfBaseComponent
{
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();
	[Parameter] public TfBookmark? ActiveBookmark { get; set; } = null;
	[Parameter] public TfBookmark? ActiveSavedUrl { get; set; } = null;

	private bool _open = false;
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _bookmarkEdit()
	{
		if(ActiveBookmark is null) return;

		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewBookmarkManageDialog>(
				ActiveBookmark,
				new ()
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

	private async Task _bookmarkRemove()
	{
		try
		{
		
			if (ActiveBookmark is null) 
				return;

			TfService.DeleteBookmark(ActiveBookmark.Id);

			ToastService.ShowSuccess(LOC("Bookmark removed"));
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