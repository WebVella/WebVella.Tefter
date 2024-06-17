using Boz.Tefter.Actions;

namespace Boz.Tefter;
public partial class BozAppInit : TfBaseComponent
{
    [Inject] protected IState<ScreenState> ScreenState { get; set; }

    private bool _inited = false;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if(firstRender) {
            initialize();
        }
    }

    private void initialize(){
        if (_inited) return;
        _inited = true;
        var bozComp = new ScreenRegionComponent
        {
            Id = new Guid("2988f4f9-6328-41f6-a8dc-1b34a1f82370"),
            ComponentType = typeof(BozViewAction),
            Position = 1,
            Region = ScreenRegionType.SpaceViewActions
        };

		var bozInlineComp = new ScreenRegionComponent
		{
			Id = new Guid("f68e757f-6e11-440d-b8d9-6c10cbcb758b"),
			ComponentType = typeof(BozViewMenuItem),
			Position = 1,
			Region = ScreenRegionType.SpaceViewMenuItems
		};
        

		Dispatcher.Dispatch(new AddComponentToRegionAction(new List<ScreenRegionComponent>{ bozComp, bozInlineComp}));
	}

}