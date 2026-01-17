namespace WebVella.Tefter.EmailSender.Components;

public partial class EmailSenderLogPageManageDialog : TfFormBaseComponent, IDialogContentComponent<EmailSenderLogPageManageDialogContext>
{
	[Parameter] public EmailSenderLogPageManageDialogContext Content { get; set; } = null!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;

	private EmailSenderLogSpacePageComponentOptions _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_form = Content.Options with { CustomIdValue = Content.Options.CustomIdValue };
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
			if (_form.RelatedIdValueType == EmailSenderLogRelatedIdValueType.AnyRelatedId
			    || _form.RelatedIdValueType == EmailSenderLogRelatedIdValueType.AnyRelatedId)
			{
				if (!Guid.TryParse(_form.CustomIdValue, out _))
				{
					MessageStore.Add(EditContext.Field(nameof(_form.CustomIdValue)), LOC("invalid GUID"));
				}
			}	
			
			
			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = TfService.UpdateSpacePageComponentOptions(Content.SpacePageId,JsonSerializer.Serialize(_form));
			ToastService.ShowSuccess(LOC("Settings successfully updated!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
			 	payload: new TfSpacePageUpdatedEventPayload(result));						
			await Dialog.CloseAsync();
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

}

public record EmailSenderLogPageManageDialogContext
{
	public Guid SpacePageId { get; set; }
	public EmailSenderLogSpacePageComponentOptions Options { get; set; } = null!;
}