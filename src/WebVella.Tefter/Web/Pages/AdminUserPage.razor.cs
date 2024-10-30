namespace WebVella.Tefter.Web.Pages;
public partial class AdminUserPage : TfBasePage
{
	[Parameter] public Guid UserId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}


}