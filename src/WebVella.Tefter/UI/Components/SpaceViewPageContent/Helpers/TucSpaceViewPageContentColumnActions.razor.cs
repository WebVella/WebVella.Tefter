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
	private async Task _addColumn()
	{
		short? nextPostition = Column.Position is not null ? (short)(Column.Position! + 1) : null;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
			new TfSpaceViewColumn() with { SpaceViewId = Column!.SpaceViewId, Position = nextPostition},
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
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

	private async Task _moveColumn(bool isLeft)
	{
		try
		{
			if (isLeft)
				await TfService.MoveSpaceViewColumnUp(Column.Id);
			else
				await TfService.MoveSpaceViewColumnDown(Column.Id);
			ToastService.ShowSuccess(LOC("Column moved!"));
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

	private async Task _deleteColumn()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column removed?")))
			return;
		try
		{
			await TfService.DeleteSpaceViewColumn(Column.Id);
			ToastService.ShowSuccess(LOC("Column removed!"));
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