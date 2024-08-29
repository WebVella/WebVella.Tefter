namespace WebVella.Tefter.Web.Pages;
public partial class SpaceDataPage : TfBasePage
{
	[Parameter] public string Menu { get; set; } = string.Empty;
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceDataId { get; set; }
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	private TucSpaceData SpaceData = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);

		SpaceData.DataProviderId = new Guid("e43c7cca-f0d3-44b3-958e-35297a72d189");
	}
}