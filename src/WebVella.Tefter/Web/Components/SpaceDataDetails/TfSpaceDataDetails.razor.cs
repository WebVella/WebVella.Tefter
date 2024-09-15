namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceDataDetails : TfBaseComponent
{
	[Parameter] public string Menu { get; set; } = "";
	[Inject] protected IState<TfState> TfState { get; set; }
	[Inject] protected IStateSelection<TfState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}

}