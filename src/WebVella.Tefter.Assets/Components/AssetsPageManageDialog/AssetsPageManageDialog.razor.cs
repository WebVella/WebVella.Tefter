using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.Assets.Components;

public partial class AssetsPageManageDialog : TfFormBaseComponent, IDialogContentComponent<AssetsPageManageDialogContext>
{
	[Parameter] public AssetsPageManageDialogContext Content { get; set; } = null!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;

	private AssetsSpacePageComponentOptions _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_form = Content.Options with { FolderId = Content.Options.FolderId };
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

public record AssetsPageManageDialogContext
{
	public Guid SpacePageId { get; set; }
	public AssetsSpacePageComponentOptions Options { get; set; } = null!;
	public List<AssetsFolder> Folders { get; set; } = new();
}