namespace WebVella.Tefter.Web.Pages;
public partial class AdminSharedColumnsPage : TfBasePage
{
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}

}