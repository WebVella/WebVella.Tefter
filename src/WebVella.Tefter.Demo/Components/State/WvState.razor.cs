namespace WebVella.Tefter.Demo.Components;
public partial class WvState : ComponentBase
{
	[Parameter]
	public RenderFragment ChildContent { get; set; }
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }

	[Inject] protected NavigationManager Navigator { get; set; }

	public Guid ComponentId { get; set; } = Guid.NewGuid();

	public CultureInfo Culture { get; set; } = new CultureInfo("en-US");

	private string _errorMessage = "";
	private bool _isLoading = true;

	private User _user = new();
	private List<Space> _spaces = new();
	private Dictionary<Guid, Space> _spaceDict = new();
	private Dictionary<Guid, SpaceItem> _spaceItemDict = new();
	private Guid? _activeSpaceId = null;
	private Guid? _activeSpaceItemId = null;
	private Guid? _activeSpaceItemViewId = null;

	//LC
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_user = SampleData.GetUser();

			await _setTheme();

			_spaces = SampleData.GetSpaces();
			_spaceDict = _spaces.ToDictionary(x => x.Id);
			foreach (var space in _spaces)
			{
				_spaceDict[space.Id] = space;
				foreach (var item in space.Items)
				{
					_spaceItemDict[item.Id] = item;
				}
			}
			_isLoading = false;
			StateHasChanged();
		}
	}
	//Public
	public List<Space> GetSpaces() => _spaces;
	public void SetActiveSpaceData(Guid? spaceId, Guid? spaceItemId, Guid? spaceItemViewId)
	{
		if (spaceId is null || spaceItemId is null)
		{
			_activeSpaceId = null;
			_activeSpaceItemId = null;
			_activeSpaceItemViewId = null;
			ActiveSpaceDataChanged?.Invoke(this, new StateActiveSpaceDataChangedEventArgs
			{
				Space = null,
				SpaceItem = null,
				SpaceItemView = null,
			});
		}
		else
		{
			_activeSpaceId = spaceId;
			_activeSpaceItemId = spaceItemId;
			_activeSpaceItemViewId = spaceItemViewId;
			if (!_spaceDict.ContainsKey(_activeSpaceId.Value))
				_errorMessage = $"Space with this identifier is not found: {spaceId}";
			if (!_spaceItemDict.ContainsKey(_activeSpaceItemId.Value))
				_errorMessage = $"Space with this identifier is not found: {_activeSpaceItemId}";

			var eventArgs = new StateActiveSpaceDataChangedEventArgs
			{
				Space = _spaceDict[_activeSpaceId.Value],
				SpaceItem = _spaceItemDict[_activeSpaceItemId.Value],
				SpaceItemView = null
			};
			eventArgs.SpaceItemView = eventArgs.SpaceItem.GetActiveView(_activeSpaceItemViewId);
			ActiveSpaceDataChanged?.Invoke(this, eventArgs);

		}

	}
	public Guid? ActiveSpaceId => _activeSpaceId;
	public Guid? ActiveSpaceItemId => _activeSpaceItemId;
	public Guid? ActiveSpaceItemViewId => _activeSpaceItemViewId;
	public (Space, SpaceItem, SpaceItemView) GetActiveSpaceData()
	{
		Space space = null;
		SpaceItem spaceItem = null;
		SpaceItemView spaceItemView = null;
		if (_activeSpaceId is not null && _spaceDict.ContainsKey(_activeSpaceId.Value))
			space = _spaceDict[_activeSpaceId.Value];

		if (_activeSpaceItemId is not null && _spaceItemDict.ContainsKey(_activeSpaceItemId.Value))
			spaceItem = _spaceItemDict[_activeSpaceItemId.Value];

		if (spaceItem is not null)
			spaceItemView = spaceItem.GetActiveView(_activeSpaceItemViewId);

		return (space, spaceItem, spaceItemView);
	}
	public EventHandler<StateActiveUserChangedEventArgs> ActiveUserChanged { get; set; }
	public EventHandler<StateActiveSpaceDataChangedEventArgs> ActiveSpaceDataChanged { get; set; }
	public EventHandler<StateSpacesDataChangedEventArgs> SpacesDataChanged { get; set; }

	public async Task SetTheme(DesignThemeModes mode, OfficeColor color)
	{
        _user.ThemeMode = mode;
        _user.ThemeColor = color;
		var themeData = new ThemeLocalStorage{ 
			Mode = mode.ToString(),
			PrimaryColor = color.ToString()
		};
        await new LocalStorageService(JSRuntimeSrv).AddItem(WvConstants.ThemeLocalKey,JsonSerializer.Serialize(themeData));
        await InvokeAsync(StateHasChanged);
	}
	public User GetUser() => _user;

	public void SetUser(User user){
        _user =user;
        ActiveUserChanged?.Invoke(this, new StateActiveUserChangedEventArgs{ User = user });
    }

	//private
	private async Task _setTheme()
	{

		var themeStorage = await new LocalStorageService(JSRuntimeSrv).GetItem(WvConstants.ThemeLocalKey);
		DesignThemeModes? mode = null;
		OfficeColor? color = null;
		if (!String.IsNullOrWhiteSpace(themeStorage))
		{
			var themeData = JsonSerializer.Deserialize<ThemeLocalStorage>(themeStorage);
			if (themeData != null)
			{
				if (Enum.TryParse(themeData.Mode,true, out DesignThemeModes outMode)) mode = outMode;
				if (Enum.TryParse(themeData.PrimaryColor, true, out OfficeColor outColor)) color = outColor;
			}
		}
        _user.ThemeMode = mode.HasValue ? mode.Value : _user.ThemeMode;
        _user.ThemeColor = color.HasValue ? color.Value : _user.ThemeColor;
	}
}
