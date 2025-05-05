namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Space.SpaceManage.TfSpaceManage", "WebVella.Tefter")]
public partial class TfSpaceManage : TfBaseComponent
{
	[Parameter] public string Menu { get; set; } = "";
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private bool _submitting = false;

	private TucRole _adminRole
	{
		get
		{
			return TfAppState.Value.UserRoles.Single(x => x.Id == TfConstants.ADMIN_ROLE_ID);
		}
	}
	private List<TucRole> _roleOptions
	{
		get
		{
			var allRolesWithoutAdmin = TfAppState.Value.UserRoles.Where(x => x.Id != TfConstants.ADMIN_ROLE_ID).ToList();
			if (TfAppState.Value.Space.Roles.Count == 0) return allRolesWithoutAdmin;
			return allRolesWithoutAdmin.Where(x => !TfAppState.Value.Space.Roles.Any(u => x.Id == u.Id)).ToList();
		}
	}
	private TucRole _selectedRole = null;
	public Guid? _removingRoleId = null;

	private async Task _editSpace()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceManageDialog>(
		TfAppState.Value.Space,
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
			UC.DeleteSpace(TfAppState.Value.Space.Id);
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
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
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
			List<TucSpaceNode> submitResult = UC.DeleteSpaceNode(node);
			ToastService.ShowSuccess(LOC("Space node deleted!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceNodes = submitResult
			}
			));
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
			List<TucSpaceNode> submitResult = UC.MoveSpaceNode(args.Item1, args.Item2);

			ToastService.ShowSuccess(LOC("Space node updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceNodes = submitResult
			}
			));
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
			List<TucSpaceNode> submitResult = UC.CopySpaceNode(nodeId);

			ToastService.ShowSuccess(LOC("Space node updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceNodes = submitResult
			}
			));
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
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var nodes = (List<TucSpaceNode>)result.Data;
			ToastService.ShowSuccess(LOC("Space page successfully updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceNodes = nodes
			}
			));
		}
	}

	private async Task _addRole()
	{
		if (_submitting) return;

		if (_selectedRole is null) return;
		try
		{
			_submitting = true;
			var result = await UC.AddRoleToSpaceAsync(_selectedRole.Id, TfAppState.Value.Space.Id);
			_updateSpaceInState(result);
			ToastService.ShowSuccess(LOC("Space role added"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			_selectedRole = null;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _removeRole(TucRole role)
	{
		if (_removingRoleId is not null) return;
		if (role is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role unassigned?")))
			return;
		try
		{
			_removingRoleId = role.Id;
			var result = await UC.RemoveRoleFromSpaceAsync(role.Id, TfAppState.Value.Space.Id);
			_updateSpaceInState(result);
			ToastService.ShowSuccess(LOC("Space role removed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_removingRoleId = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _setPrivacy(bool newValue)
	{
		try
		{
			var result = UC.SetSpacePrivacy(TfAppState.Value.Space.Id, newValue);
			_updateSpaceInState(result);
			ToastService.ShowSuccess(LOC("Space access changed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_removingRoleId = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _updateSpaceInState(TucSpace space)
	{
		var state = TfAppState.Value with { Space = space };
		var recIndex = TfAppState.Value.CurrentUserSpaces.FindIndex(x => x.Id == space.Id);
		if (recIndex > 0)
		{
			var userSpaces = state.CurrentUserSpaces.ToList();
			userSpaces[recIndex] = space;
			state = state with { CurrentUserSpaces = userSpaces };
		}
		Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
	}


}