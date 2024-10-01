namespace WebVella.Tefter.Web.Components;
public partial class TfPagesContent : TfBaseComponent
{
	[Parameter] public string Slug { get; set; } = String.Empty;
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

	private Dictionary<string, object> _getComponentContext()
	{
		var componentData = new Dictionary<string, object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfScreenRegionComponentContext{
			Hash = Guid.NewGuid(),
		};

		return componentData;
	}
}