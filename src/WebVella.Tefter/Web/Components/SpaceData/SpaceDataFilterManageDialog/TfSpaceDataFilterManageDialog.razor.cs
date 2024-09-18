namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceDataFilterManageDialog.TfSpaceDataFilterManageDialog","WebVella.Tefter")]
public partial class TfSpaceDataFilterManageDialog : TfFormBaseComponent, IDialogContentComponent<TucFilterBase>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucFilterBase Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) Content = new();
		base.InitForm(Content);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? new Icons.Regular.Size20.Add() : new Icons.Regular.Size20.Save();
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
			//if (String.IsNullOrWhiteSpace(UC.Form.Password))
			//	UC.Form.Password = null; //fixes a case when password was touched
			//if (UC.Form.Password != UC.Form.ConfirmPassword)
			//{
			//	MessageStore.Add(EditContext.Field(nameof(UC.Form.Password)), LOC("Passwords do not match"));
			//}
			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new Result<TucUser>();
			//if (_isCreate)
			//{
			//	result = await UC.CreateUserWithFormAsync();
			//}
			//else
			//{
			//	result = await UC.UpdateUserWithFormAsync();
			//}

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
			{
				await Dialog.CloseAsync(result.Value);
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

}
