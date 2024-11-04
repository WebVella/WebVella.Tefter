namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.HeaderNavigation.TfHeaderNavigation", "WebVella.Tefter")]
public partial class TfHeaderNavigation : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private Guid? _opendedNode = null;
	private Guid? _activeContextMenuId = null;

	private void _toggleMenu(Guid id, bool? isOpen = null)
	{
		if (isOpen is null)
		{
			if (_opendedNode == id) _opendedNode = null;
			else _opendedNode = id;
		}
		else if (!isOpen.Value)
		{
			_opendedNode = null;
		}
	}

	private async Task _onClick(Guid? presetId)
	{
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PresetIdQueryName] = presetId;
		await Navigator.ApplyChangeToUrlQuery(queryDict);

	}

	private async Task _onRightClick(Guid? presetId)
	{
		_activeContextMenuId = presetId;
	}

	private async Task _onEdit(Guid presetId)
	{

	}

	private async Task _onRemove(Guid presetId)
	{

	}

	private async Task _onAdd(Guid presetId)
	{

	}

	private Dictionary<Guid, List<Guid>> _generateSelectionDict()
	{
		var dict = new Dictionary<Guid, List<Guid>>();
		foreach (var item in TfAppState.Value.SpaceView.Presets)
		{
			_getChildNodeIds(item, dict);
		}
		return dict;
	}

	private List<Guid> _getChildNodeIds(TucSpaceViewPreset item, Dictionary<Guid, List<Guid>> dict)
	{
		var list = new List<Guid>();
		list.Add(item.Id);
		foreach (var node in item.Nodes)
		{
			list.Add(node.Id);
			var childIds = _getChildNodeIds(node, dict);
			list.AddRange(childIds);
		}
		dict[item.Id] = list.ToList();
		return list;
	}
}