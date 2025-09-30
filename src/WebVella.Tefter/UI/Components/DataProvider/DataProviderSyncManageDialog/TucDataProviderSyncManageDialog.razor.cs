namespace WebVella.Tefter.UI.Components;
public partial class TucDataProviderSyncManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProvider?>
{
	[Parameter] public TfDataProvider? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "Update synchronization schedule";
	private string _btnText = "Save";
	private Icon _iconBtn = TfConstants.GetIcon("Save")!;
	private TfDataProviderUpdateSyncForm _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
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

			var provider = TfUIService.UpdateDataProviderSunchronization(_form.Id,_form.SynchScheduleMinutes,_form.SynchScheduleEnabled);
			ToastService.ShowSuccess(LOC("Provider synchronization is updated"));
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

