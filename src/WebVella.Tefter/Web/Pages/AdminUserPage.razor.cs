namespace WebVella.Tefter.Web.Pages;
public partial class AdminUserPage : TfBasePage
{
	[Parameter] public Guid UserId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IStateSelection<TfState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}


}