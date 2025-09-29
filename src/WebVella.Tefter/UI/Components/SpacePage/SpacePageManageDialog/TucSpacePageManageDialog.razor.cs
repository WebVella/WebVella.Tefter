namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpacePage?>
{
	[Inject] protected ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] protected ITfMetaService TfMetaService { get; set; } = default!;
	[Parameter] public TfSpacePage? Content { get; set; } = null;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;
	private TfSpacePage _form = new();
	private TfSpacePage? _parentNode = null;
	private IEnumerable<TfSpacePage> _parentNodeOptions = Enumerable.Empty<TfSpacePage>();
	private ReadOnlyCollection<TfSpacePageAddonMeta> _pageComponents = default!;
	private TfSpacePageAddonMeta? _selectedPageComponent = null;
	private DynamicComponent typeSettingsComponent = default!;
	private TfSpace? _space = null;
	private List<Option<bool>> _copyOptions = new();
	private List<Option<Guid>> _allPageOptions = new();
	private Option<bool> _copyOption = default!;
	private Option<Guid>? _copyPage = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init();
	}

	private void _init()
	{
		if (Content is null) throw new Exception("Content is null");
		if (Content.SpaceId == Guid.Empty) throw new Exception("Space Id is required");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create page") : LOC("Manage page");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;

		var spaceDict = TfSpaceUIService.GetSpaces().ToDictionary(x => x.Id);
		var allPages = TfSpaceUIService.GetAllSpacePages().Where(x => x.Type == TfSpacePageType.Page).ToList();

		foreach (var page in allPages)
		{
			_allPageOptions.Add(new Option<Guid>()
			{
				Value = page.Id,
				Icon = (TfConstants.GetIcon(page.FluentIconName!)!, Color.Accent, "start"),
				Text = $"{spaceDict[page.SpaceId].Name} > {page.Name}"
			});
		}
		
		_copyOptions.Add(new Option<bool>
		{
			Value = false,
			Text = LOC("create from blank")
		});
		if (_allPageOptions.Any())
		{
			_allPageOptions = _allPageOptions.OrderBy(x => x.Text).ToList();
			_copyPage = _allPageOptions[0];
			_copyOptions.Add(new Option<bool>
			{
				Value = true,
				Text = LOC("use existing page")
			});
		}
		_copyOption = _copyOptions[0];

		_pageComponents = TfMetaService.GetSpacePagesComponentsMeta();
		_space = spaceDict[Content.SpaceId];
		_parentNodeOptions = _getParents();
		if (_isCreate)
		{
			_form = new()
			{
				Id = Guid.NewGuid(),
				SpaceId = Content.SpaceId,
				Type = TfSpacePageType.Page,
				FluentIconName = "Document",
				ComponentId = _pageComponents.Count > 0 ? _pageComponents[0].ComponentId : null
			};
		}
		else
		{

			_form = new()
			{
				Id = Content.Id,
				SpaceId = Content.SpaceId,
				Type = TfSpacePageType.Page,
				ChildPages = Content.ChildPages,
				ComponentId = Content.ComponentId,
				ComponentOptionsJson = Content.ComponentOptionsJson,
				ComponentType = Content.ComponentType,
				FluentIconName = Content.FluentIconName,
				Name = Content.Name,
				ParentId = Content.ParentId,
				ParentPage = Content.ParentPage,
				Position = Content.Position,
			};
		}
		
		if (_form.ComponentId.HasValue)
		{
			_selectedPageComponent = _pageComponents.FirstOrDefault(x => x.ComponentId == _form.ComponentId);
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
		ITfSpacePageAddon addonComponent = null;
		TfSpacePage submit = _form with { Id = _form.Id };
		if (_copyPage is null)
		{
			if (submit.Type == TfSpacePageType.Folder)
			{
				submit.ComponentOptionsJson = null;
				submit.ComponentType = null;
				submit.ComponentId = null;

			}
			else if (submit.Type == TfSpacePageType.Page)
			{
				if (typeSettingsComponent is not null)
				{
					addonComponent = typeSettingsComponent.Instance as ITfSpacePageAddon;
					settingsErrors = addonComponent.ValidateOptions();
					submit.ComponentOptionsJson = addonComponent.GetOptions();
					submit.ComponentType = _selectedPageComponent?.Instance.GetType();
					submit.ComponentId = _selectedPageComponent?.Instance.AddonId;
				}
			}
		}
		else
		{
			submit.TemplateId = _copyPage?.Value;
		}
		//Check form
		var isValid = EditContext.Validate();
		if (!isValid || settingsErrors.Count > 0) return;


		_isSubmitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			if (_parentNode is null) submit.ParentId = null;
			else submit.ParentId = _parentNode.Id;
			TfSpacePage newPage = default!;
			if (_isCreate)
			{
				newPage = TfSpaceUIService.CreateSpacePage(
					page: submit
					);
				ToastService.ShowSuccess(LOC("Space page created!"));
			}
			else
			{
				newPage = TfSpaceUIService.UpdateSpacePage(submit);
				ToastService.ShowSuccess(LOC("Space page updated!"));
			}

			_parentNodeOptions = _getParents();
			await Dialog.CloseAsync(newPage);
		}
		catch (Exception ex)
		{
			ProcessFormSubmitException(ex, EditContext, MessageStore);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}


	}

	private IEnumerable<TfSpacePage> _getParents()
	{
		var parents = new List<TfSpacePage>();
		var spacePages = TfSpaceUIService.GetSpacePages(Content.SpaceId);
		foreach (var item in spacePages)
		{
			_fillParents(parents, item, new List<Guid> { _form.Id });
		}
		return parents.AsEnumerable();
	}

	private void _fillParents(List<TfSpacePage> parents, TfSpacePage current, List<Guid> ignoreNodes)
	{
		//if (current.Type == TfSpaceNodeType.Folder && !ignoreNodes.Contains(current.Id)) parents.Add(current);
		if (!ignoreNodes.Contains(current.Id)) parents.Add(current);
		foreach (var item in current.ChildPages) _fillParents(parents, item, ignoreNodes);
	}

	private void _selectedParentChanged(TfSpacePage node)
	{
		_parentNode = node;
		_form.ParentId = node?.Id;
	}

	private void _typeChanged(TfSpacePageType type)
	{
		_form.Type = type;
		if (type == TfSpacePageType.Folder && _form.FluentIconName == "Document")
			_form.FluentIconName = "Folder";
		else if (type == TfSpacePageType.Page && _form.FluentIconName == "Folder")
			_form.FluentIconName = "Document";

		if (type == TfSpacePageType.Folder) _selectedPageComponent = null;
		else if (type == TfSpacePageType.Page && _pageComponents.Any()) _selectedPageComponent = _pageComponents[0];
	}
	private void _pageComponentChanged(TfSpacePageAddonMeta meta)
	{
		_selectedPageComponent = meta;
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		if (_selectedPageComponent is null) return dict;

		var context = new TfSpacePageAddonContext();
		context.Icon = _form.FluentIconName;
		context.Space = _space;
		context.ComponentOptionsJson = _form.ComponentOptionsJson;
		context.Mode = _isCreate ? TfComponentMode.Create : TfComponentMode.Update;
		dict["Context"] = context;
		return dict;
	}
}
