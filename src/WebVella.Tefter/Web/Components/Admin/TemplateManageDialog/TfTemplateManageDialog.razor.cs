﻿
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateManageDialog.TfTemplateManageDialog", "WebVella.Tefter")]
public partial class TfTemplateManageDialog : TfFormBaseComponent, IDialogContentComponent<TucTemplate>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucTemplate Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TucManageTemplateModel _form = null;
	private List<ITfTemplateProcessor> _processors = new();
	private ITfTemplateProcessor _selectedProcessor = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create template") : LOC("Manage template");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add") : TfConstants.GetIcon("Save");
		_processors = TfAppState.Value.AdminTemplateProcessors;
		_form = new TucManageTemplateModel
		{
			Id = Content.Id,
			Description = Content.Description,
			ContentProcessorType = Content.ContentProcessorType,
			FluentIconName = Content.FluentIconName,
			IsEnabled = Content.IsEnabled,
			IsSelectable = Content.IsSelectable,
			Name = Content.Name,
			SettingsJson = Content.SettingsJson,
			UserId = TfAppState.Value.CurrentUser.Id
		};
		if (_form.ContentProcessorType is not null && _form.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessor)) != null)
		{
			_selectedProcessor = (ITfTemplateProcessor)Activator.CreateInstance(_form.ContentProcessorType);

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

		base.InitForm(_form);

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{

			await InvokeAsync(StateHasChanged);
		}

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
			TucManageTemplateModel submit = _form with { UserId = TfAppState.Value.CurrentUser.Id };
			if (_isCreate)
				submit = submit with { Id = Guid.NewGuid() };
			Result<TucTemplate> submitResult = null;
			if (_isCreate) submitResult = UC.CreateTemplate(submit);
			else submitResult = UC.UpdateTemplate(submit);


			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				await Dialog.CloseAsync(submitResult.Value);
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
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}
	private void _componentChanged(ITfTemplateProcessor item)
	{
		_selectedProcessor = item;
		_form.ContentProcessorType = _selectedProcessor.GetType();
	}
}

