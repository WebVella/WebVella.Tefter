namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Space.SpaceManage.TfSpaceManage", "WebVella.Tefter")]
public partial class TfSpaceManage : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private bool _submitting = false;
	private string _selectedName = null;
	private TfSpaceNodeType _selectedType = TfSpaceNodeType.Page;
	private TucSpaceNode _selectedParent = null;

	private string _icon = "Document";

	private async Task _editSpace()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceManageDialog>(
		TfAppState.Value.Space,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpace)result.Data;
			ToastService.ShowSuccess(LOC("Space successfully updated!"));
			//Change user state > spaces
			var userSpaces = TfAppState.Value.CurrentUserSpaces.ToList();
			var itemIndex = userSpaces.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				userSpaces[itemIndex] = item;
			}
			var state = TfAppState.Value with { CurrentUserSpaces = userSpaces };
			if (TfAppState.Value.Space is not null
				&& TfAppState.Value.Space.Id == item.Id)
			{
				state = state with { Space = item };
			}

			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: state
			));
		}
	}

	private async Task _deleteSpace()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;

		try
		{
			var result = UC.DeleteSpace(TfAppState.Value.Space.Id);

			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				var spaceList = TfAppState.Value.CurrentUserSpaces.Where(x => x.Id != TfAppState.Value.Space.Id).ToList();
				Dispatcher.Dispatch(new SetAppStateAction(
									component: this,
									state: TfAppState.Value with
									{
										CurrentUserSpaces = spaceList
									}
								));
				Navigator.NavigateTo(TfConstants.HomePageUrl);
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

	private async Task _addNode()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceNodeManageDialog>(
		new TucSpaceNode(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var nodes = (List<TucSpaceNode>)result.Data;
			ToastService.ShowSuccess(LOC("Space page successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceNodes = nodes
			}
			));
		}

	}

	private async Task _removeNode(TucSpaceNode node)
	{
		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			Result<List<TucSpaceNode>> submitResult = UC.DeleteSpaceNode(node);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space node deleted!"));
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceNodes = submitResult.Value
				}
				));
			}
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
	private async Task _moveNode(Tuple<TucSpaceNode, bool> args)
	{
		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			Result<List<TucSpaceNode>> submitResult = UC.MoveSpaceNode(args.Item1, args.Item2);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space node updated!"));
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceNodes = submitResult.Value
				}
				));
			}
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

	private async Task _copyNode(Guid nodeId)
	{
		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			Result<List<TucSpaceNode>> submitResult = UC.CopySpaceNode(nodeId);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space node updated!"));
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceNodes = submitResult.Value
				}
				));
			}
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

	private async Task _editNode(Guid nodeId)
	{
		var node = TfAppState.Value.SpaceNodes.GetSpaceNodeById(nodeId);
		if (node == null)
		{
			ToastService.ShowError(LOC("Space node not found"));
			return;
		}
		var dialog = await DialogService.ShowDialogAsync<TfSpaceNodeManageDialog>(
		node,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var nodes = (List<TucSpaceNode>)result.Data;
			ToastService.ShowSuccess(LOC("Space page successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceNodes = nodes
			}
			));
		}
	}


}