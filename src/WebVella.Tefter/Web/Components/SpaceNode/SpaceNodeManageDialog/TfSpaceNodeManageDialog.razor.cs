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
	private DynamicComponent typeSettingsComponent;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create page") : LOC("Manage page");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		_pageComponents = TfMetaProvider.GetSpaceNodesComponentsMeta();
		if (_isCreate)
		{
			_form = _form with
			{
				Id = Guid.NewGuid(),
				SpaceId = TfAppState.Value.Space.Id,
				Type = TfSpaceNodeType.Page,
				Icon = TfConstants.PageIconString,
				ComponentTypeFullName = _pageComponents.Count > 0 ? _pageComponents[0].ComponentType.FullName : null
			};
		}
		else
		{

			_form = Content with { Id = Content.Id };
			_parentIdString = _form.ParentId.ToString();
		}
		if (!String.IsNullOrWhiteSpace(_form.ComponentTypeFullName))
		{
			_selectedPageComponent = _pageComponents.FirstOrDefault(x => x.ComponentType.FullName == _form.ComponentTypeFullName);
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

		//Get dynamic settings component errors
		List<ValidationError> settingsErrors = new();
		ITfSpaceNodeComponent addonComponent = null;
		TucSpaceNode submit = _form with { Id = _form.Id };
		if (submit.Type == TfSpaceNodeType.Folder)
		{
			submit.ComponentOptionsJson = null;
			submit.ComponentTypeFullName = null;

		}
		else if (submit.Type == TfSpaceNodeType.Page)
		{
			if (typeSettingsComponent is not null)
			{
				addonComponent = typeSettingsComponent.Instance as ITfSpaceNodeComponent;
				settingsErrors = addonComponent.ValidateOptions();
				submit.ComponentOptionsJson = addonComponent.GetOptions();
				submit.ComponentTypeFullName = _selectedPageComponent?.ComponentType.FullName;
			}
		}

		//Check form
		var isValid = EditContext.Validate();
		if (!isValid || settingsErrors.Count > 0) return;


		_isSubmitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			Result<List<TucSpaceNode>> submitResult = null;


			if (String.IsNullOrWhiteSpace(_parentIdString)) submit.ParentId = null;
			else submit.ParentId = new Guid(_parentIdString);

			if (_isCreate) submitResult = UC.CreateSpaceNode(submit);
			else submitResult = UC.UpdateSpaceNode(submit);

			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space page created!"));
				//Reload spaceViews and spaceData in case new ones were created
				var spaceViews = UC.GetSpaceViewList(_form.SpaceId);
				var spaceData = UC.GetSpaceDataList(_form.SpaceId);
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceNodes = submitResult.Value,
					SpaceViewList = spaceViews,
					SpaceDataList = spaceData
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
			_fillParents(parents, item, new List<Guid> { _form.Id });
		}
		return parents.AsEnumerable();
	}

	private void _fillParents(List<TucSpaceNode> parents, TucSpaceNode current, List<Guid> ignoreNodes)
	{
		//if (current.Type == TfSpaceNodeType.Folder && !ignoreNodes.Contains(current.Id)) parents.Add(current);
		if (!ignoreNodes.Contains(current.Id)) parents.Add(current);
		foreach (var item in current.ChildNodes) _fillParents(parents, item, ignoreNodes);
	}

	private void _typeChanged(TfSpaceNodeType type)
	{
		_form.Type = type;
		if (type == TfSpaceNodeType.Folder && _form.Icon == TfConstants.PageIconString)
			_form.Icon = TfConstants.FolderIconString;
		else if (type == TfSpaceNodeType.Page && _form.Icon == TfConstants.FolderIconString)
			_form.Icon = TfConstants.PageIconString;

		if (type == TfSpaceNodeType.Folder) _selectedPageComponent = null;
		else if (type == TfSpaceNodeType.Page && _pageComponents.Any()) _selectedPageComponent = _pageComponents[0];
	}
	private void _pageComponentChanged(TfSpaceNodeComponentMeta meta)
	{
		_selectedPageComponent = meta;
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		if (_selectedPageComponent is null) return dict;

		var context = new TfSpaceNodeComponentContext();
		context.Icon = _form.Icon;
		context.SpaceId = TfAppState.Value.Space.Id;
		context.ComponentOptionsJson = _form.ComponentOptionsJson;
		context.Mode = TfComponentMode.Create;
		dict["Context"] = context;
		return dict;
	}
}
