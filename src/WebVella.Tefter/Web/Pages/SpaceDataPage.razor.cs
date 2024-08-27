namespace WebVella.Tefter.Web.Pages;
public partial class SpaceDataPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceDataId { get; set; }
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	private TucSpaceData SpaceData = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);

		SpaceData.Filters.Add(new TucFilterBoolean { ColumnName = "boz", ComparisonMethod = TfFilterBooleanComparisonMethod.Equal, Value = true });
	}


}