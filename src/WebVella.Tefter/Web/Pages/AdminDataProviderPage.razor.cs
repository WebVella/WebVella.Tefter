namespace WebVella.Tefter.Web.Pages;
public partial class AdminDataProviderPage : TfBasePage
{
	[Parameter] public Guid ProviderId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IStateSelection<TfState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}



}