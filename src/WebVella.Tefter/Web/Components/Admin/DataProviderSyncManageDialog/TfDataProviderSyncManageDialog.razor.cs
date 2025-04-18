namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderSyncManageDialog.TfDataProviderSyncManageDialog", "WebVella.Tefter")]
public partial class TfDataProviderSyncManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProvider>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucDataProvider Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "Update synchronization schedule";
	private string _btnText = "Save";
	private Icon _iconBtn = TfConstants.SaveIcon;
	private TucDataProviderUpdateSyncForm _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initForm();
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		_initForm();
	}

	private void _initForm()
	{
		if (Content is null) throw new Exception("Content is null");
		_form = _form with
		{
			Id = Content.Id,
			SynchScheduleEnabled = Content.SynchScheduleEnabled,
			SynchScheduleMinutes = Content.SynchScheduleMinutes,
		};
		base.InitForm(_form);
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			MessageStore.Clear();

			//Get dynamic settings component errors
			//Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TucDataProvider provider;
			provider = UC.UpdateDataProviderSunchronization(_form);
			await Dialog.CloseAsync(provider);
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
}

