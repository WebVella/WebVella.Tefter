namespace WebVella.Tefter.Talk.Components;

public partial class TalkPageManageDialog : TfFormBaseComponent, IDialogContentComponent<TalkPageManageDialogContext>
{
	[Parameter] public TalkPageManageDialogContext Content { get; set; } = null!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;

	private TalkSpacePageComponentOptions _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_form = Content.Options with { ChannelId = Content.Options.ChannelId };
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
			// if (String.IsNullOrWhiteSpace(_form.CountSharedColumnName))
			// {
			// 	MessageStore.Add(EditContext.Field(nameof(_form.CountSharedColumnName)), LOC("required"));
			// }

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			TfService.UpdateSpacePageComponentOptions(Content.SpacePageId,JsonSerializer.Serialize(_form));
			ToastService.ShowSuccess(LOC("Settings successfully updated!"));
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

public record TalkPageManageDialogContext
{
	public Guid SpacePageId { get; set; }
	public TalkSpacePageComponentOptions Options { get; set; } = null!;
	public List<TalkChannel> Channels { get; set; } = new();
}