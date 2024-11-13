namespace WebVella.Tefter.Talk.Components;
//[TfScreenRegionComponent(TfScreenRegion.SpaceViewToolbarActions,1,null,null)]
public partial class TalkSpaceViewToolbarAction : TucBaseScreenRegionComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
}