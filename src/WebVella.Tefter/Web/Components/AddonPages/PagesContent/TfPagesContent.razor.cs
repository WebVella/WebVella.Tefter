namespace WebVella.Tefter.Web.Components;
public partial class TfPagesContent : TfBaseComponent
{
	[Parameter] public Guid ItemId { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }
	[Inject] protected IStateSelection<TfUserState,string> ThemeSidebarStyle { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
		ThemeSidebarStyle.Select(x => x.ThemeSidebarStyle);
	}

	private Dictionary<string, object> _getComponentContext()
	{
		var componentData = new Dictionary<string, object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TucScreenRegionComponentContext{
			Hash = Guid.NewGuid(),
		};

		return componentData;
	}
}