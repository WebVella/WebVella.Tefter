namespace WebVella.Tefter.Web.Components.AppBar;
public partial class TfAppBar : TfBaseComponent
{
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	[Parameter] public List<MenuItem> Items { get; set; } = new();
	[Parameter] public string Style { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}
}