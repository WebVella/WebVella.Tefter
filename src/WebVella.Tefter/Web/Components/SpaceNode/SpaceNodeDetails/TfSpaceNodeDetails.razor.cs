namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceNode.SpaceNodeDetails.TfSpaceNodeDetails", "WebVella.Tefter")]
public partial class TfSpaceNodeDetails : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }

	private bool _isRemoving = false;
	//private bool _showManageButtons = false;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}
	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		if (TfAppState.Value.SpaceNode is not null)
		{
			dict["Context"] = new TfSpacePageAddonContext
			{
				ComponentOptionsJson = TfAppState.Value.SpaceNode.ComponentOptionsJson,
				Icon = TfAppState.Value.SpaceNode.Icon,
				Mode = TfComponentMode.Read,
				SpaceId = TfAppState.Value.SpaceNode.SpaceId,
				SpacePageId = TfAppState.Value.SpaceNode.Id,
				EditNode = EventCallback.Factory.Create(this, _onEdit),
				DeleteNode = EventCallback.Factory.Create(this, _onRemove)
			};
		}


		return dict;
	}

	private async Task _onRemove()
	{
		if (TfAppState.Value.SpaceNode is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space node removed?")))
			return;

		if (_isRemoving) return;

		_isRemoving = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			List<TucSpaceNode> submitResult = UC.DeleteSpaceNode(TfAppState.Value.SpaceNode);
			ToastService.ShowSuccess(LOC("Space node deleted!"));
			Navigator.NavigateTo(string.Format(TfConstants.SpaceNodePageUrl, TfAppState.Value.Space.Id, null), true);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isRemoving = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _onEdit()
	{
		if (TfAppState.Value.SpaceNode is null) return;

		var node = TfAppState.Value.SpaceNodes.GetSpaceNodeById(TfAppState.Value.SpaceNode.Id);
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
			ToastService.ShowSuccess(LOC("Space page successfully saved!"));
			Navigator.NavigateTo(string.Format(TfConstants.SpaceNodePageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceNode.Id), true);
		}
	}

}