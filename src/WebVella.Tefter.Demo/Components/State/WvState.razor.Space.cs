namespace WebVella.Tefter.Demo.Components;
public partial class WvState : ComponentBase
{
	public List<Space> GetSpaces() => _spaces;
	public void SetActiveSpaceData(Guid? spaceId, Guid? spaceDataId, Guid? spaceViewId)
	{
		if (_spaces.Count > 0)
		{
			//Always have to have active space
			if (spaceId is null) spaceId = _spaces.OrderBy(x => x.Position).First().Id;

			if (spaceDataId is null) spaceDataId = _spaceDict[spaceId.Value].DataItems.OrderBy(x => x.Position).First().Id;

			if (spaceViewId is null) spaceViewId = _spaceDataDict[spaceDataId.Value].Views.OrderBy(x => x.Position).First().Id;
		}

		if (spaceId is null || spaceDataId is null)
		{
			_activeSpaceId = null;
			_activeSpaceDataId = null;
			_activeSpaceViewId = null;
			ActiveSpaceDataChanged?.Invoke(this, new StateActiveSpaceDataChangedEventArgs
			{
				Space = null,
				SpaceData = null,
				SpaceView = null,
			});
		}
		else
		{
			_activeSpaceId = spaceId;
			_activeSpaceDataId = spaceDataId;
			_activeSpaceViewId = spaceViewId;
			if (!_spaceDict.ContainsKey(_activeSpaceId.Value))
				_errorMessage = $"Space with this identifier is not found: {spaceId}";
			if (!_spaceDataDict.ContainsKey(_activeSpaceDataId.Value))
				_errorMessage = $"Space with this identifier is not found: {_activeSpaceDataId}";

			var eventArgs = new StateActiveSpaceDataChangedEventArgs
			{
				Space = _spaceDict[_activeSpaceId.Value],
				SpaceData = _spaceDataDict[_activeSpaceDataId.Value],
				SpaceView = null
			};
			eventArgs.SpaceView = eventArgs.SpaceData.GetActiveView(_activeSpaceViewId);
			ActiveSpaceDataChanged?.Invoke(this, eventArgs);

		}

	}
	public Guid? ActiveSpaceId => _activeSpaceId;
	public Guid? ActiveSpaceDataId => _activeSpaceDataId;
	public Guid? ActiveSpaceViewId => _activeSpaceViewId;
	public ActiveSpaceMeta GetActiveSpaceMeta()
	{
		ActiveSpaceMeta meta = new();
		if (_activeSpaceId is not null && _spaceDict.ContainsKey(_activeSpaceId.Value))
			meta.Space = _spaceDict[_activeSpaceId.Value];

		if (_activeSpaceDataId is not null && _spaceDataDict.ContainsKey(_activeSpaceDataId.Value))
			meta.SpaceData = _spaceDataDict[_activeSpaceDataId.Value];

		if (meta.SpaceData is not null)
			meta.SpaceView = meta.SpaceData.GetActiveView(_activeSpaceViewId);
		return meta;
	}
	public EventHandler<StateActiveSpaceDataChangedEventArgs> ActiveSpaceDataChanged { get; set; }

	public void SpaceOnLocationChangeHandler(object sender, LocationChangedEventArgs e)
	{
		var (spaceId, spaceDataId, spaceViewId) = _initSpaceDataFromUrl(e.Location);
		if (spaceId == _activeSpaceId && spaceDataId == _activeSpaceDataId && spaceViewId == _activeSpaceViewId) return;
		SetActiveSpaceData(spaceId, spaceDataId, spaceViewId);
	}

	private (Guid?, Guid?, Guid?) _initSpaceDataFromUrl(string url)
	{
		if (string.IsNullOrWhiteSpace(url)) return (null, null, null);
		url = url.Trim().ToLowerInvariant();
		var uri = new Uri(url);

		if (!uri.LocalPath.StartsWith("/space/")) return (null, null, null);

		Guid? spaceId = null;
		Guid? spaceDataId = null;
		Guid? spaceViewId = null;

		var nodes = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

		if (nodes.Length >= 2)
		{
			if (Guid.TryParse(nodes[1], out Guid outGuid)) spaceId = outGuid;
		}

		if (nodes.Length >= 4)
		{
			if (Guid.TryParse(nodes[3], out Guid outGuid)) spaceDataId = outGuid;
		}

		if (nodes.Length >= 6)
		{
			if (Guid.TryParse(nodes[5], out Guid outGuid)) spaceViewId = outGuid;
		}

		return (spaceId, spaceDataId, spaceViewId);

	}


}
