namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceNodeDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}
	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		if (TfAppState.Value.SpaceNode is not null)
		{
			dict["Context"] = new TfSpacePageAddonContext
			{
				ComponentOptionsJson = TfAppState.Value.SpaceNode.ComponentOptionsJson,
				Icon = TfAppState.Value.SpaceNode.Icon,
				Mode = TfComponentMode.Read,
				SpaceId = TfAppState.Value.SpaceNode.SpaceId,
				SpacePageId = TfAppState.Value.SpaceNode.Id
			};
		}


		return dict;
	}

}