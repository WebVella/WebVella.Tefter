namespace WebVella.Tefter.Demo.Components;
public partial class WvState : ComponentBase, IAsyncDisposable
{
	[Parameter]
	public RenderFragment ChildContent { get; set; }
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }
	[Inject] protected IWvService WvService { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }

	public Guid ComponentId { get; set; } = Guid.NewGuid();

	public CultureInfo Culture { get; set; } = new CultureInfo("en-US");

	private string _errorMessage = "";
	private bool _isLoading = true;

	private User _user = new();
	private UISettings _uiSettings = null;
	private List<Space> _spaces = new();
	private Dictionary<Guid, Space> _spaceDict = new();
	private Dictionary<Guid, SpaceDataset> _spaceDataDict = new();
	private Dictionary<Guid, SpaceView> _spaceViewDict = new();
	private Guid? _activeSpaceId = null;
	private Guid? _activeSpaceDataId = null;
	private Guid? _activeSpaceViewId = null;
	private bool _sidebarExpanded = true;

	//LC
	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= OnLocationChanged;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_user = WvService.GetUserByCookieValue(String.Empty);

			await _setTheme();

			_spaces = WvService.GetAllSpaces();
			_spaceDict = _spaces.ToDictionary(x => x.Id);
			foreach (var space in _spaces)
			{
				_spaceDict[space.Id] = space;
				foreach (var data in space.DataItems)
				{
					_spaceDataDict[data.Id] = data;

					foreach (var view in data.Views)
					{
						_spaceViewDict[view.Id] = view;
					}
				}
			}
			var (spaceId, spaceDataId, spaceViewId) = _initSpaceDataFromUrl(Navigator.Uri);
			SetActiveSpaceData(spaceId, spaceDataId, spaceViewId);

			_isLoading = false;
			await InvokeAsync(StateHasChanged);
			Navigator.LocationChanged += OnLocationChanged;
		}
	}
	//Public
	public EventHandler<StateActiveUserChangedEventArgs> ActiveUserChanged { get; set; }

	public EventHandler<StateUISettingsChangedEventArgs> UISettingsChanged { get; set; }

	public UISettings GetUiSettings() => _uiSettings;
	public async Task SetTheme(DesignThemeModes mode, OfficeColor color)
	{
		_user.ThemeMode = mode;
		_user.ThemeColor = color;
		_uiSettings.Mode = mode.ToString();
		_uiSettings.PrimaryColor = color.ToString();
		await new LocalStorageService(JSRuntimeSrv).AddItem(WvConstants.UISettingsLocalKey, JsonSerializer.Serialize(_uiSettings));
		UISettingsChanged?.Invoke(this, new StateUISettingsChangedEventArgs { });
		await InvokeAsync(StateHasChanged);
	}

	public async Task ToggleSidebar()
	{
		_uiSettings.SidebarExpanded = !_sidebarExpanded;
		await new LocalStorageService(JSRuntimeSrv).AddItem(WvConstants.UISettingsLocalKey, JsonSerializer.Serialize(_uiSettings));
		_sidebarExpanded = _uiSettings.SidebarExpanded;
		UISettingsChanged?.Invoke(this, new StateUISettingsChangedEventArgs { });
		await InvokeAsync(StateHasChanged);
	}

	public User GetUser() => _user;

	public void SetUser(User user)
	{
		_user = user;
		ActiveUserChanged?.Invoke(this, new StateActiveUserChangedEventArgs { User = user });
	}

	//private

	private void OnLocationChanged(object sender, LocationChangedEventArgs e)
	{
		base.InvokeAsync(async () =>
		{
			await Task.Delay(1);
			SpaceOnLocationChangeHandler(sender, e);
			FilterOnLocationChangeHandler(sender, e);
		});
	}

	private async Task _setTheme()
	{

		var themeStorage = await new LocalStorageService(JSRuntimeSrv).GetItem(WvConstants.UISettingsLocalKey);
		DesignThemeModes? mode = null;
		OfficeColor? color = null;
		if (!String.IsNullOrWhiteSpace(themeStorage))
		{
			_uiSettings = JsonSerializer.Deserialize<UISettings>(themeStorage);
			if (_uiSettings != null)
			{
				if (Enum.TryParse(_uiSettings.Mode, true, out DesignThemeModes outMode)) mode = outMode;
				if (Enum.TryParse(_uiSettings.PrimaryColor, true, out OfficeColor outColor)) color = outColor;
			}
		}
		if (_uiSettings is null) _uiSettings = new();
		_user.ThemeMode = mode.HasValue ? mode.Value : _user.ThemeMode;
		_user.ThemeColor = color.HasValue ? color.Value : _user.ThemeColor;
		_sidebarExpanded = _uiSettings.SidebarExpanded;
	}


}
