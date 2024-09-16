namespace WebVella.Tefter.Web.Components;
public partial class TfAppBar : TfBaseComponent
{
	[Inject] protected IStateSelection<TfState, bool> ScreenStateSidebarExpanded { get; set; }
	[Parameter] public List<MenuItem> Items { get; set; } = new();
	[Parameter] public string Style { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}

}