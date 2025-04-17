namespace WebVella.Tefter.Web.Pages;
public partial class AdminUserPage : TfBasePage
{
	[Parameter] public Guid UserId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (UserId == Guid.Empty && TfAppState.Value.AdminUsers is not null
			&& TfAppState.Value.AdminUsers.Count > 0)
		{
			Navigator.NavigateTo(string.Format(TfConstants.AdminUserDetailsPageUrl, TfAppState.Value.AdminUsers[0].Id));
		}
	}
}