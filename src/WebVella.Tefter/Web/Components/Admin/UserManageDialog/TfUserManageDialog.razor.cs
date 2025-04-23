namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.UserManageDialog.TfUserManageDialog", "WebVella.Tefter")]
public partial class TfUserManageDialog : TfFormBaseComponent, IDialogContentComponent<TucUser>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Parameter] public TucUser Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TucUserAdminManageForm _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		base.InitForm(_form);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create user") : LOC("Manage user");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;

		if (_isCreate)
		{

			_form.Culture = TfConstants.CultureOptions[0];
		}
		else
		{

			_form = new TucUserAdminManageForm()
			{
				ConfirmPassword = null,
				Password = null,
				Email = Content.Email,
				Enabled = Content.Enabled,
				FirstName = Content.FirstName,
				LastName = Content.LastName,
				Id = Content.Id,
				ThemeMode = Content.Settings.ThemeMode,
				ThemeColor = Content.Settings.ThemeColor,
				IsSidebarOpen = Content.Settings.IsSidebarOpen,
			};
			if (Content.Roles is not null)
			{
				_form.Roles = Content.Roles.ToList();
			}

			_form.Culture = TfConstants.CultureOptions.FirstOrDefault(x => x.CultureCode == Content.Settings.CultureName);
			if (_form.Culture is null) _form.Culture = TfConstants.CultureOptions[0];
		}
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

			var result = new TucUser();
			
			if (_isCreate)
			{
				result = await UC.CreateUserWithFormAsync(_form);
			}
			else
			{
				result = await UC.UpdateUserWithFormAsync(_form);
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
