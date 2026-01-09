using WebVella.Tefter.Models;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePagesDialog : TfBaseComponent, IDialogContentComponent<Guid>
{
	[Parameter] public Guid Content { get; set; } = Guid.Empty;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private TfNavigationState _navState = null!;
	private TfSpace _space = null!;
	private List<TfSpacePage> _spacePages = null!;
	public bool _submitting = false;

	protected override void OnInitialized()
	{
		if (Content == Guid.Empty) throw new Exception("Content is null");
		_init();
	}

	private void _init(TfSpace? space = null)
	{
		if (space is null)
			_space = TfService.GetSpace(Content);
		else
			_space = space;

		_spacePages = TfService.GetSpacePages(_space.Id);
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _addPage()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
		new TfSpacePage() { SpaceId = _space.Id },
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

	private async Task _removePage(TfSpacePage node)
	{
		if (_submitting) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this page deleted?")))
			return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			TfService.DeleteSpacePage(node.Id);
			_init();
			ToastService.ShowSuccess(LOC("Space page deleted!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageDeletedEventPayload(node));				
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
	private async Task _movePage(Tuple<TfSpacePage, bool> args)
	{
		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			TfService.MoveSpacePage(args.Item1.Id, args.Item2);
			_init();
			ToastService.ShowSuccess(LOC("Space page updated!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageUpdatedEventPayload(args.Item1));				
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

	private async Task _copyPage(Guid nodeId)
	{
		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			var (pageId, pages) = TfService.CopySpacePage(nodeId);
			_init();
			ToastService.ShowSuccess(LOC("Space page updated!"));
			var page = TfService.FindPageById(pageId,pages)!;
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpacePageCreatedEventPayload(page));					
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

	private async Task _editPage(Guid nodeId)
	{
		var node = _spacePages.GetSpaceNodeById(nodeId);
		if (node == null)
		{
			ToastService.ShowError(LOC("Space page not found"));
			return;
		}
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
		node,
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
			_init();
		}
	}

}
