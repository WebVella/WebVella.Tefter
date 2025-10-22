namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContentColumnActions : TfBaseComponent
{
	[Parameter] public TfSpaceViewColumn Column { get; set; } = null!;
	[Parameter] public TfUser User { get; set; } = null!;

	private bool _open = false;

	private void _clickHandler()
	{
		_open = !_open;
	}
	
	private async Task _manageColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
			Column,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}	
}