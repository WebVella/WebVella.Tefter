namespace WebVella.Tefter.Templates.Components;
[LocalizationResource("WebVella.Tefter.Templates.Components.TemplateCreateDialog.TemplateCreateDialog", "WebVella.Tefter")]
public partial class TemplateCreateDialog : TfFormBaseComponent, IDialogContentComponent<Template>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public ITemplatesService Service { get; set; }
	[Parameter] public Template Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private DynamicComponent typeSettingsComponent;
	private Template _form = new();
	private List<ITemplateProcessor> _processors = new();
	private ITemplateProcessor _selectedProcessor = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Create template");
		_btnText = LOC("Create");
		_iconBtn = TfConstants.AddIcon;
		if (TfAuxDataState.Value.Data.ContainsKey(TemplatesConstants.TEMPLATE_APP_PROCESSORS_LIST_DATA_KEY))
		{
			_processors = (List<ITemplateProcessor>)TfAuxDataState.Value.Data[TemplatesConstants.TEMPLATE_APP_PROCESSORS_LIST_DATA_KEY];
		}
		if (_processors.Count > 0)
		{
			_selectedProcessor = _processors[0];
			_form.ContentProcessorType = _selectedProcessor.GetType();

		}
		else{ 
			_error = LOC("No content processors found");
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
			var submit = new CreateTemplateModel{ 
				Name = _form.Name,
				ContentProcessorType = _form.ContentProcessorType
			};
			Result<Template> submitResult = Service.CreateTemplate(submit);

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
	private void _componentChanged(ITemplateProcessor item){ 
		_selectedProcessor = item;
		_form.ContentProcessorType= _selectedProcessor.GetType();
	}
}

