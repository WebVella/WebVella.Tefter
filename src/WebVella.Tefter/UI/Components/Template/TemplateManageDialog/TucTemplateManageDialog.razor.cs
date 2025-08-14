
namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateManageDialog.TfTemplateManageDialog", "WebVella.Tefter")]
public partial class TucTemplateManageDialog : TfFormBaseComponent, IDialogContentComponent<TfTemplate?>
{
	[Inject] public ITfTemplateUIService TfTemplateUIService { get; set; } = default!;
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Parameter] public TfTemplate? Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;
	private TfManageTemplateModel _form = new();
	private ReadOnlyCollection<ITfTemplateProcessorAddon> _processors = default!;
	private TfUser _currentUser = default!;
	private ITfTemplateProcessorAddon? _selectedProcessor = null;

	private List<TfSpaceDataAsOption> _spaceDataAll = new();
	private List<TfSpaceDataAsOption> _spaceDataOptions = new();
	private List<TfSpaceDataAsOption> _spaceDataSelection = new();
	private TfSpaceDataAsOption? _spaceDataOption = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create template") : LOC("Manage template");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")!.WithColor(Color.Neutral) : TfConstants.GetIcon("Save")!.WithColor(Color.Neutral);
		_processors = TfTemplateUIService.GetProcessors();
		_currentUser = (await TfUserUIService.GetCurrentUserAsync())!;
		_form = new TfManageTemplateModel
		{
			Id = Content.Id,
			Description = Content.Description,
			ContentProcessorType = Content.ContentProcessorType,
			FluentIconName = Content.FluentIconName,
			IsEnabled = Content.IsEnabled,
			IsSelectable = Content.IsSelectable,
			Name = Content.Name,
			SettingsJson = Content.SettingsJson,
			UserId = _currentUser.Id,
			SpaceDataList = Content.SpaceDataList,
		};
		if (_form.ContentProcessorType is not null && _form.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
		{
			_selectedProcessor = (ITfTemplateProcessorAddon?)Activator.CreateInstance(_form.ContentProcessorType);

		}
		if (_selectedProcessor is null)
		{
			if (_processors.Count > 0)
			{
				_selectedProcessor = _processors.FirstOrDefault(x => x.ResultType == Content.ResultType);
				if (_selectedProcessor is null)
					_selectedProcessor = _processors[0];

				_form.ContentProcessorType = _selectedProcessor.GetType();

			}
			else
			{
				_error = LOC("No content processors found");
			}
		}
		if (String.IsNullOrWhiteSpace(_form.FluentIconName) && _selectedProcessor is not null)
		{
			_form.FluentIconName = _selectedProcessor.ResultType.GetFluentIcon();
		}
		_spaceDataAll = TfTemplateUIService.GetSpaceDataOptionsForTemplate();
		_recalcSpaceDataOptions();
		base.InitForm(_form);
	}


	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);

			MessageStore.Clear();
			////Check form
			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TfManageTemplateModel submit = _form with { UserId = _currentUser.Id };
			if (_isCreate)
				submit = submit with { Id = Guid.NewGuid() };

			TfTemplate template = default!;
			if (_isCreate)
			{
				template = TfTemplateUIService.CreateTemplate(submit);
				ToastService.ShowSuccess(LOC("Template successfully created"));
			}
			else
			{
				template = TfTemplateUIService.UpdateTemplate(submit);
				ToastService.ShowSuccess(LOC("Template successfully updated"));
			}

			await Dialog.CloseAsync(template);
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}
	private void _componentChanged(ITfTemplateProcessorAddon item)
	{
		_selectedProcessor = item;
		_form.ContentProcessorType = _selectedProcessor.GetType();
	}

	private void _recalcSpaceDataOptions()
	{
		_spaceDataSelection = new();
		_spaceDataOptions = _spaceDataAll.ToList();
		foreach (var item in _form.SpaceDataList)
		{
			var attachment = _spaceDataAll.Where(x => x.Id == item).FirstOrDefault();
			if (attachment is null) continue;
			_spaceDataSelection.Add(attachment);
			_spaceDataOptions = _spaceDataOptions.Where(x => x.Id != item).ToList();
		}
	}

	private void _spaceDataOptionChanged(TfSpaceDataAsOption option)
	{
		if (option is null) return;

		_spaceDataOption = null;
		bool isSelected = _form.SpaceDataList.Contains(option.Id);
		if (isSelected) return;
		_form.SpaceDataList.Add(option.Id);
		_recalcSpaceDataOptions();
	}

	private void _removeItem(TfSpaceDataAsOption item)
	{
		var index = _form.SpaceDataList.FindIndex(x => x == item.Id);
		if (index == -1) return;
		_form.SpaceDataList.RemoveAt(index);
		_recalcSpaceDataOptions();
	}
}

