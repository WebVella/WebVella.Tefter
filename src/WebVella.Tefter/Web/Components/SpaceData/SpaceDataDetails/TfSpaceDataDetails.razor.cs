namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceDataDetails : TfBaseComponent
{
	[Parameter] public string Menu { get; set; } = "";
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

}