using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Components.UserManageDialog;
public partial class TfUserManageDialog : TfFormBaseComponent, IDialogContentComponent<User>
{
	[Inject] private IState<SystemState> SystemState {  get; set; }
	[Parameter] public User Content { get; set; }

	[CascadingParameter]
	public FluentDialog Dialog { get; set; }

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private TfUserManageDialogModel _form = new();
	private List<Role> _allRoles = new();
	private bool _isCreate = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create user") : LOC("Manage user");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? new Icons.Regular.Size20.Add() : new Icons.Regular.Size20.Save();
		base.InitForm(_form);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _loadDataAsync()
	{
		_isBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			_allRoles = SystemState.Value.Roles;

			if (_isCreate)
			{

				_form.Culture = TfConstants.CultureOptions[0];
			}
			else
			{

				_form = new TfUserManageDialogModel()
				{
					ConfirmPassword = null,
					Password = null,
					Email = Content.Email,
					Enabled = Content.Enabled,
					FirstName = Content.FirstName,
					LastName = Content.LastName,
					Id = Content.Id,
					Roles = Content.Roles.ToList(),
					ThemeMode = Content.Settings.ThemeMode,
					ThemeColor = Content.Settings.ThemeColor,
					IsSidebarOpen = Content.Settings.IsSidebarOpen,
				};

				_form.Culture = TfConstants.CultureOptions.FirstOrDefault(x => x.CultureCode == Content.Settings.CultureName);
				if (_form.Culture is null) _form.Culture = TfConstants.CultureOptions[0];
			}
			base.InitForm(_form);
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
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
			if (String.IsNullOrWhiteSpace(_form.Password))
				_form.Password = null; //fixes a case when password was touched
			if (_form.Password != _form.ConfirmPassword)
			{
				MessageStore.Add(EditContext.Field(nameof(_form.Password)), LOC("Passwords do not match"));
			}
			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			UserBuilder userBuilder;
			if (_isCreate)
			{
				userBuilder = IdentityManager.CreateUserBuilder(null);
				userBuilder
					.WithEmail(_form.Email)
					.WithFirstName(_form.FirstName)
					.WithLastName(_form.LastName)
					.WithPassword(_form.Password)
					.Enabled(_form.Enabled)
					.CreatedOn(DateTime.Now)
					.WithThemeMode(_form.ThemeMode)
					.WithThemeColor(_form.ThemeColor)
					.WithOpenSidebar(true)
					.WithCultureCode(_form.Culture.CultureInfo.Name)
					.WithRoles(_form.Roles.ToArray());
			}
			else
			{
				userBuilder = IdentityManager.CreateUserBuilder(Content);
				userBuilder
					.WithEmail(_form.Email)
					.WithFirstName(_form.FirstName)
					.WithLastName(_form.LastName)
					.Enabled(_form.Enabled)
					.WithThemeMode(_form.ThemeMode)
					.WithThemeColor(_form.ThemeColor)
					.WithCultureCode(_form.Culture.CultureInfo.Name)
					.WithRoles(_form.Roles.ToArray());
			}

			var user = userBuilder.Build();
			var result = await IdentityManager.SaveUserAsync(user);
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

	private void _roleChanged(Role role)
	{
		if (_form.Roles.Any(x => x.Id == role.Id))
		{
			_form.Roles = _form.Roles.Where(x => x.Id != role.Id).ToList();
		}
		else
		{
			_form.Roles.Add(role);
		}
	}
}

public class TfUserManageDialogModel
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	[EmailAddress]
	public string Email { get; set; }
	[Required]
	public string FirstName { get; set; }
	[Required]
	public string LastName { get; set; }
	internal string Password { get; set; }
	internal string ConfirmPassword { get; set; }
	[Required]
	public bool Enabled { get; set; } = true;
	[Required]
	public DesignThemeModes ThemeMode { get; set; } = DesignThemeModes.System;
	[Required]
	public OfficeColor ThemeColor { get; set; } = OfficeColor.Excel;
	[Required]
	public bool IsSidebarOpen { get; set; } = true;
	public CultureOption Culture { get; set; }
	public List<Role> Roles { get; set; } = new();

}