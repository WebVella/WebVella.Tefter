namespace WebVella.Tefter.Web.Components.Navigation;
public partial class TfNavigation: TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}
	private void _addSpaceHandler(){
		ToastService.ShowToast(ToastIntent.Warning, "Will open add new space modal");
	}

}