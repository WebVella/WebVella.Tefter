namespace WebVella.Tefter.Web.Components.ManageUserDialog;
public partial class TfManageUserDialog : TfFromBaseComponent, IDialogContentComponent<User>
{
    [Parameter]
    public User Content { get; set; }

    [CascadingParameter]
    public FluentDialog Dialog { get; set; }

    private bool _isBusy = true;
    private bool _isSubmitting = false;
    private string _title = "";
    private Icon _icon;
    private TfManageUserDialogModel _form = new();
    private List<Role> _allRoles = new();


    protected override void OnInitialized()
    {
        base.OnInitialized();
        base.InitForm(_form);
        if(Content.Id == Guid.Empty){ 
            _title = LC["Create user"];
            _icon = new Icons.Regular.Size20.Add();
        }
        else{
            _title = LC["Manage user"];
            _icon = new Icons.Regular.Size20.Edit();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if(firstRender){
            var rolesResult = await IdentityManager.GetRolesAsync();

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

public class TfManageUserDialogModel{
    [Required]
	public Guid Id { get; init; }
	[Required]
    [EmailAddress]
	public string Email { get; init; }
    [Required]
	public string FirstName { get; init; }
    [Required]
	public string LastName { get; init; }
    [Required]
	internal string Password { get; init; }
	[Required]
	internal string ConfirmPassword { get; init; }
	[Required]
	public bool Enabled { get; init; } = true;
	public DateTime CreatedOn { get; init; }
	public UserSettings Settings { get; init; } = new();
	public List<Role> Roles { get; init; } = new();

}