namespace WebVella.Tefter.UI.Components;

public partial class TucUserThemeDialog : TfBaseComponent, IDialogContentComponent<TfUser?>
{
	[Parameter] public TfUser? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private bool _isSubmitting = false;
	private DesignThemeModes _mode = DesignThemeModes.System;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_mode = Content.Settings?.ThemeMode ?? DesignThemeModes.System;
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new TfUser();

			result = await TfService.UpdateUserThemeModeAsync(Content!.Id,_mode);
			ToastService.ShowSuccess(LOC("User account was successfully updated!"));

			await Dialog.CloseAsync(result);
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