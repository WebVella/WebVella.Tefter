
namespace WebVella.Tefter.UI.Components;
public partial class TucTemplateManageDialog : TfFormBaseComponent, IDialogContentComponent<TfTemplate?>
{
	[Parameter] public TfTemplate? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;
	private bool _isCreate = false;
	private TfManageTemplateModel _form = new();
	private ReadOnlyCollection<ITfTemplateProcessorAddon> _processors = null!;
	private ITfTemplateProcessorAddon? _selectedProcessor = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create template") : LOC("Manage template");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")!.WithColor(Color.Neutral) : TfConstants.GetIcon("Save")!.WithColor(Color.Neutral);
		_processors = TfService.GetTemplateProcessors();
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
			UserId = TfAuthLayout.GetState().User.Id,
			RequiredColumnsList = Content.RequiredColumnsList,
			ColumnNamePreprocess = Content.ColumnNamePreprocess,
		};
		if (_form.ContentProcessorType is not null 
			&& _form.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
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
		InitForm(_form);
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
			TfManageTemplateModel submit = _form with { UserId = TfAuthLayout.GetState().User.Id };
			if (_isCreate)
				submit = submit with { Id = Guid.NewGuid() };

			TfTemplate template = null!;
			if (_isCreate)
			{
				template = TfService.CreateTemplate(submit);
				ToastService.ShowSuccess(LOC("Template successfully created"));
				await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
					payload: new TfTemplateCreatedEventPayload(template));					
			}
			else
			{
				template = TfService.UpdateTemplate(submit);
				ToastService.ShowSuccess(LOC("Template successfully updated"));
				await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
					payload: new TfTemplateUpdatedEventPayload(template));					
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
	
}

