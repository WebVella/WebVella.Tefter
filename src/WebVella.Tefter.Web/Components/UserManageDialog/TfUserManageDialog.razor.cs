namespace WebVella.Tefter.Web.Components.UserManageDialog;
public partial class TfUserManageDialog : TfFormBaseComponent, IDialogContentComponent<User>
{
	[Parameter]
	public User Content { get; set; }

	[CascadingParameter]
	public FluentDialog Dialog { get; set; }

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private Icon _icon;
	private TfUserManageDialogModel _form = new();
	private List<Role> _allRoles = new();


	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
		if (Content is null)
		{
			_title = LC["Create user"];
			_icon = new Icons.Regular.Size20.Add();
		}
		else
		{
			_title = LC["Manage user"];
			_icon = new Icons.Regular.Size20.Edit();
			_form = new TfUserManageDialogModel()
			{
				ConfirmPassword = null,
				Password = null,
				CreatedOn = Content.CreatedOn,
				Email = Content.Email,
				Enabled = Content.Enabled,
				FirstName = Content.FirstName,
				LastName = Content.LastName,
				Id = Content.Id,
				Roles = Content.Roles.ToList(),
				Settings = Content.Settings,
			};
		}
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();

		}
	}

	private async Task _loadDataAsync()
	{
		_isBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var rolesResult = await IdentityManager.GetRolesAsync();
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
		}
		finally
		{
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _save()
	{
		User newUser = new User();
		await Dialog.CloseAsync(newUser);
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
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
	[Required]
	internal string Password { get; set; }
	[Required]
	internal string ConfirmPassword { get; set; }
	[Required]
	public bool Enabled { get; set; } = true;
	public DateTime CreatedOn { get; set; }
	public UserSettings Settings { get; set; } = new();
	public List<Role> Roles { get; set; } = new();

}