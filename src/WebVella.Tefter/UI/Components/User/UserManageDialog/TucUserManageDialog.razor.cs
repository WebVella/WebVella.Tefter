namespace WebVella.Tefter.UI.Components;
public partial class TucUserManageDialog : TfFormBaseComponent, IDialogContentComponent<TfUser?>
{
	[Parameter] public TfUser? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;
	private bool _isCreate = false;
	private TfUserManageForm _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		InitForm(_form);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create user") : LOC("Manage user");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;

		if (_isCreate)
		{

			_form.Culture = TfConstants.CultureOptions[0];
		}
		else
		{
			_form = new()
			{
				ConfirmPassword = null,
				Password = null,
				Email = Content.Email,
				Enabled = Content.Enabled,
				FirstName = Content.FirstName,
				LastName = Content.LastName,
				Id = Content.Id,
				Color = Content.Settings.Color,
				ThemeMode = Content.Settings.ThemeMode,
				IsSidebarOpen = Content.Settings.IsSidebarOpen,
				Culture = TfConstants.CultureOptions.FirstOrDefault(x => x.CultureName == Content.Settings.CultureName)
			};
			_form.Culture ??= TfConstants.CultureOptions[0];
		}
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
			if (String.IsNullOrWhiteSpace(_form.Password))
				_form.Password = null; //fixes a case when password was touched
			if (_form.Password != _form.ConfirmPassword)
			{
				MessageStore.Add(EditContext.Field(nameof(_form.Password)), LOC("Passwords do not match"));
			}
			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			TfUser result;
			
			if (_isCreate)
			{
				result = await TfService.CreateUserWithFormAsync(_form);
				ToastService.ShowSuccess(LOC("User account was successfully created!"));
			}
			else
			{
				result = await TfService.UpdateUserWithFormAsync(_form);
				ToastService.ShowSuccess(LOC("User account was successfully updated!"));
			}
			await Dialog.CloseAsync(result);
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
