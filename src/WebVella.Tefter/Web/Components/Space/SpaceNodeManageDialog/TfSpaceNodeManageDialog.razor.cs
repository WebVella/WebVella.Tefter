namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceNodeManageDialog.TfSpaceNodeManageDialog", "WebVella.Tefter")]
public partial class TfSpaceNodeManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceNode>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected ITfMetaProvider TfMetaProvider { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucSpaceNode Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TucSpaceNode _form = new();
	private string _parentIdString = null;
	private ReadOnlyCollection<TfSpaceNodeComponentMeta> _pageComponents;
	private TfSpaceNodeComponentMeta _selectedPageComponent = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create space node") : LOC("Manage space node");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		_pageComponents = TfMetaProvider.GetSpaceNodesComponentsMeta();
		if (_isCreate)
		{
			_form = _form with { Id = Guid.NewGuid(), SpaceId = TfAppState.Value.Space.Id };
		}
		else
		{

			_form = Content with { Id = Content.Id };
			_parentIdString = _form.ParentId.ToString();
			if(!String.IsNullOrWhiteSpace(_form.ComponentType)){ 
				
			}
		}

		base.InitForm(_form);
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		if (_isSubmitting) return;

		MessageStore.Clear();

		if (!EditContext.Validate()) return;

		_isSubmitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			Result<List<TucSpaceNode>> submitResult = null;
			if(String.IsNullOrWhiteSpace(_parentIdString)) _form.ParentId = null;
			else _form.ParentId = new Guid(_parentIdString);
			if (_isCreate) submitResult = UC.CreateSpaceNode(_form);
			else submitResult = UC.UpdateSpaceNode(_form);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space page created!"));
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceNodes = submitResult.Value
				}
				));
				await _cancel();
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}


	}

	private IEnumerable<TucSpaceNode> _getParents()
	{
		var parents = new List<TucSpaceNode>();
		foreach (var item in TfAppState.Value.SpaceNodes)
		{
			_fillParents(parents, item,new List<Guid>{ _form.Id});
		}
		return parents.AsEnumerable();
	}

	private void _fillParents(List<TucSpaceNode> parents, TucSpaceNode current,List<Guid> ignoreNodes)
	{
		if (current.Type == TfSpaceNodeType.Folder && !ignoreNodes.Contains(current.Id)) parents.Add(current);
		foreach (var item in current.ChildNodes) _fillParents(parents, item,ignoreNodes);
	}

	private void _typeChanged(TfSpaceNodeType type){ 
		_form.Type = type;
		if(type == TfSpaceNodeType.Folder && _form.Icon == TfConstants.PageIconString)
			_form.Icon = TfConstants.FolderIconString;
		else if(type == TfSpaceNodeType.Page && _form.Icon == TfConstants.FolderIconString)
			_form.Icon = TfConstants.PageIconString;
	}

}
