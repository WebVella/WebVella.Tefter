namespace WebVella.Tefter.Web.Components.SpaceDataDetails;
public partial class TfSpaceDataDetails : TfBaseComponent
{
	[Parameter] public string Menu { get; set; } = "";
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}

}