namespace WebVella.Tefter.Web.Pages;
public partial class AdminDataDetailsPage : TfBasePage
{
	[Parameter] public Guid ProviderId { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}



}