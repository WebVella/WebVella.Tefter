namespace WebVella.Tefter.Web.Pages;
public partial class AdminRolePage : TfBasePage
{
	[Parameter] public Guid RoleId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RoleId == Guid.Empty && TfAppState.Value.UserRoles is not null
			&& TfAppState.Value.UserRoles.Count > 0)
		{
			Navigator.NavigateTo(string.Format(TfConstants.AdminRoleDetailsPageUrl, TfAppState.Value.UserRoles[0].Id));
		}
	}
}