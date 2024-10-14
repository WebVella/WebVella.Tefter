namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewHeaderNavigation.TfSpaceViewHeaderNavigation", "WebVella.Tefter")]
public partial class TfSpaceViewHeaderNavigation : TfBaseComponent
{
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private Guid? _opendedNode = null;

	private void _toggleMenu(Guid id, bool? isOpen = null)
	{
		if (isOpen is null)
		{
			if (_opendedNode == id) _opendedNode = null;
			else _opendedNode = id;
		}
		else if(!isOpen.Value){ 
			_opendedNode = null;
		}
	}

	private async Task _onClick(Guid? presetId){ 
		var queryDict = new Dictionary<string,object>();
		queryDict[TfConstants.PresetIdQueryName] = presetId;
		await Navigator.ApplyChangeToUrlQuery(queryDict);

	}
}