namespace WebVella.Tefter.UI.Components;

public partial class TucPresetFiltersCard : TfBaseComponent
{
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;

	[Parameter] public TfDataset Dataset { get; set; } = null!;

	[Parameter] public List<TfSpaceViewPreset> Items { get; set; } = new();

	[Parameter] public EventCallback<List<TfSpaceViewPreset>> ItemsChanged { get; set; }

	[Parameter] public string? Title { get; set; } = null;

	[Parameter] public string? Style { get; set; } = null;

	private bool _submitting = false;


	private async Task _addPreset()
	{
		var context = new TfPresetFilterManagementContext
		{
			Item = null,
			DateSet = Dataset,
			SpaceView = SpaceView
		};
		var dialog = await DialogService.ShowDialogAsync<TucPresetFilterManageDialog>(
			context,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	private async Task _removePreset(Guid nodeId)
	{
		_submitting = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			await TfService.RemoveSpaceViewPreset(SpaceView.Id, nodeId);
			ToastService.ShowSuccess(LOC("Preset successfully copied!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpaceViewUpdatedEventPayload(SpaceView));			
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _movePreset(Tuple<Guid, bool> args)
	{
		_submitting = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			await TfService.MoveSpaceViewPreset(SpaceView.Id, args.Item1, args.Item2);
			ToastService.ShowSuccess(LOC("Preset successfully moved!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpaceViewUpdatedEventPayload(SpaceView));			
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _copyPreset(Guid presetId)
	{
		_submitting = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			await TfService.CopySpaceViewPreset(SpaceView.Id, presetId);
			ToastService.ShowSuccess(LOC("Preset successfully copied!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpaceViewUpdatedEventPayload(SpaceView));					
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editPreset(Guid presetId)
	{
		var context = new TfPresetFilterManagementContext
		{
			Item = Items.GetPresetById(presetId), 
			DateSet = Dataset, 
			SpaceView = SpaceView
		};
		var dialog = await DialogService.ShowDialogAsync<TucPresetFilterManageDialog>(
			context,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}




}

public enum TfPresetFilterItemType
{
	[Description("preset filter")] PresetFilter = 0,
	[Description("filter group")] Group = 1
}