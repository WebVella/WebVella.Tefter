namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.UserManageDialog.TfUserManageDialog","WebVella.Tefter")]
public partial class TfUserManageDialog : TfFormBaseComponent, IDialogContentComponent<TucUser>
{
	[Inject] private UserAdminUseCase UC { get; set; }
	[Parameter] public TucUser Content { get; set; }
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
		await UC.Init(this.GetType());
		base.InitForm(UC.Form);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create user") : LOC("Manage user");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? new Icons.Regular.Size20.Add() : new Icons.Regular.Size20.Save();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (_isCreate)
			{

				UC.Form.Culture = TfConstants.CultureOptions[0];
			}
			else
			{

				UC.Form = new TucUserAdminManageForm()
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
					foreach (var role in Content.Roles)
					{
						var listIndex = UC.AllRoles.FindIndex(x => x.Id == role.Id);
						if (listIndex != -1) UC.Form.Roles.Add(UC.AllRoles[listIndex]);
					}
				}

				UC.Form.Culture = TfConstants.CultureOptions.FirstOrDefault(x => x.CultureCode == Content.Settings.CultureName);
				if (UC.Form.Culture is null) UC.Form.Culture = TfConstants.CultureOptions[0];
			}
			base.InitForm(UC.Form);
			await InvokeAsync(StateHasChanged);
		}
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
			if (String.IsNullOrWhiteSpace(UC.Form.Password))
				UC.Form.Password = null; //fixes a case when password was touched
			if (UC.Form.Password != UC.Form.ConfirmPassword)
			{
				MessageStore.Add(EditContext.Field(nameof(UC.Form.Password)), LOC("Passwords do not match"));
			}
			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new Result<TucUser>();
			if (_isCreate)
			{
				result = await UC.CreateUserWithFormAsync();
			}
			else
			{
				result = await UC.UpdateUserWithFormAsync();
			}

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
