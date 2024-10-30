namespace WebVella.Tefter.Web.Components;
public partial class TfAdminPagesContent : TfBaseComponent
{
	[Parameter] public string Slug { get; set; } = String.Empty;
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
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