namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateCreateDialog.TfTemplateCreateDialog", "WebVella.Tefter")]
public partial class TfTemplateCreateDialog : TfFormBaseComponent, IDialogContentComponent<TucTemplate>
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
	private TucManageTemplateModel _form = new();
	private List<ITfTemplateProcessor> _processors = new();
	private ITfTemplateProcessor _selectedProcessor = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Create template");
		_btnText = LOC("Create");
		_iconBtn = TfConstants.AddIcon;
		_processors = TfAppState.Value.AdminTemplateProcessors;
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
			var submit = _form with {Id = Guid.NewGuid()};
			Result<TucTemplate> submitResult = UC.CreateTemplate(submit);

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
	private void _componentChanged(ITfTemplateProcessor item){ 
		_selectedProcessor = item;
		_form.ContentProcessorType= _selectedProcessor.GetType();
	}
}

