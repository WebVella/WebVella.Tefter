namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewManageMainTabDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceView?>
{
	[Parameter] public TfSpaceView? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private TfSpaceViewSettings _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) throw new Exception("Content Id is required");
		_form = Content.Settings with { CanCreateRows = Content.Settings.CanCreateRows };
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
			//Columns should not be generated on edit
			MessageStore.Clear();


			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			await TfService.UpdateSpaceViewSettings(Content!.Id, _form);

			await Dialog.CloseAsync();
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