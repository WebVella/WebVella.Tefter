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

	private IEnumerable<TucSpaceNode> _getParents()
	{
		var parents = new List<TucSpaceNode>();
		foreach (var item in TfAppState.Value.SpaceNodes)
		{
			_fillParents(parents, item);
		}
		return parents.AsEnumerable();
	}

	private void _fillParents(List<TucSpaceNode> parents, TucSpaceNode current)
	{
		if (current.Type == TfSpaceNodeType.Folder) parents.Add(current);
		foreach (var item in current.ChildNodes) _fillParents(parents, item);
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

	private async Task _removeNode(Guid nodeId)
	{
		//Items = _removeNode(Items, nodeId);
		//_submitting = true;
		//await InvokeAsync(StateHasChanged);
		//await ItemsChanged.InvokeAsync(Items);
		//_submitting = false;
		//await InvokeAsync(StateHasChanged);
	}
	private async Task _moveNode(Tuple<Guid, bool> args)
	{
		//Items = _moveNode(Items, args.Item1, args.Item2);
		//_submitting = true;
		//await InvokeAsync(StateHasChanged);
		//await ItemsChanged.InvokeAsync(Items);
		//_submitting = false;
		//await InvokeAsync(StateHasChanged);
	}

	private async Task _copyNode(Guid presetId)
	{
		//TucSpaceViewPreset source = ModelHelpers.GetPresetById(Items, presetId);
		//if (source is null) return;
		//if (source.ParentId is not null)
		//{
		//	TucSpaceViewPreset parent = ModelHelpers.GetPresetById(Items, source.ParentId.Value);
		//	if (parent is null) return;

		//	var sourceIndex = parent.Nodes.FindIndex(x => x.Id == source.Id);
		//	if (sourceIndex > -1)
		//	{
		//		parent.Nodes.Insert(sourceIndex + 1, _copyNode(source));
		//	}
		//}
		//else
		//{
		//	var sourceIndex = Items.FindIndex(x => x.Id == source.Id);
		//	if (sourceIndex > -1)
		//	{
		//		Items.Insert(sourceIndex + 1, _copyNode(source));
		//	}
		//}

		//_submitting = true;
		//await InvokeAsync(StateHasChanged);
		//await ItemsChanged.InvokeAsync(Items);
		//_submitting = false;
		//await InvokeAsync(StateHasChanged);
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