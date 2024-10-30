namespace WebVella.Tefter.Web.Layout;
public partial class AdminLayout : FluxorLayout
{
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }
	[Inject] protected IStateSelection<TfUserState,DesignThemeModes> ThemeMode { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
		ThemeMode.Select(x => x.ThemeMode);
	}	
	
}