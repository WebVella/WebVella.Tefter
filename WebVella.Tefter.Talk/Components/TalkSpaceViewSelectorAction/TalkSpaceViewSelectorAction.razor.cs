using Fluxor;
using Microsoft.AspNetCore.Components;
using WebVella.Tefter.Web.Store;

namespace WebVella.Tefter.Talk.Components;
[TfScreenRegionComponent(TfScreenRegion.SpaceViewSelectorActions, 10, null, null)]
public partial class TalkSpaceViewSelectorAction : TucBaseScreenRegionComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
}