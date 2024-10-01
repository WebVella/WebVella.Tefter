using Fluxor;
using Microsoft.AspNetCore.Components;
using WebVella.Tefter.Web.Store;

namespace WebVella.Tefter.Talk.Components;
[TfScreenRegionComponent(TfScreenRegion.SpaceViewToolbarActions,1,null,null)]
public partial class TalkSpaceViewToolbarAction : TucBaseScreenRegionComponent
{ 
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
}