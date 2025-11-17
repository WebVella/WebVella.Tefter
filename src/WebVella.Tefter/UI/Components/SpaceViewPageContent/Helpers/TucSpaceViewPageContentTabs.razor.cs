namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContentTabs : TfBaseComponent, IAsyncDisposable
{
	// Dependency Injection
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfSpacePage SpacePage { get; set; } = null!;

	private List<TfMenuItem> _menu = new();
	private TfNavigationState? _navState = null!;
	private Guid _initedViewId = Guid.Empty;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await TfEventProvider.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	protected override void OnParametersSet()
	{
		if (_initedViewId != SpaceView.Id)
			_init(TfAuthLayout.GetState().NavigationState);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			TfEventProvider.SpaceViewUpdatedEvent += On_SpaceViewUpdated;
		}
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
			{
				_init(TfAuthLayout.GetState().NavigationState);
				await InvokeAsync(StateHasChanged);
			}
		});
	}

	private async Task On_SpaceViewUpdated(TfSpaceViewUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			_init(TfAuthLayout.GetState().NavigationState);
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
			_menu = new();
			Guid? currentPresetId = Navigator.GetGuidFromQuery(TfConstants.PresetIdQueryName);
			Icon? mainIcon = String.IsNullOrWhiteSpace(SpaceView.Settings.MainTabFluentIcon)
				? null
				: TfConstants.GetIcon(SpaceView.Settings.MainTabFluentIcon);

			if (SpaceView.Presets.Count > 0)
			{
				_menu.Add(new TfMenuItem
				{
					Id = "tf-main",
					Text =
						String.IsNullOrWhiteSpace(SpaceView.Settings.MainTabLabel)
							? "Main"
							: SpaceView.Settings.MainTabLabel,
					Url = NavigatorExt.AddQueryValueToUri(Navigator.GetLocalUrl(), TfConstants.PresetIdQueryName,
						null),
					Selected = currentPresetId is null,
					IconCollapsed = mainIcon,
					IconExpanded = mainIcon
				});

				foreach (var prItem in SpaceView.Presets)
				{
					_menu.Add(_getMenuItem(prItem));
				}

				foreach (var menuItem in _menu)
				{
					if (menuItem.Id == "tf-main") continue;
					_setSelection(menuItem, currentPresetId);
				}
			}
		}
		finally
		{
			UriInitialized = _navState?.Uri ?? String.Empty;
			_initedViewId = SpaceView.Id;
		}
	}

	private TfMenuItem _getMenuItem(TfSpaceViewPreset preset)
	{
		var item = new TfMenuItem
		{
			Id = TfConverters.ConvertGuidToHtmlElementId(preset.Id),
			Text = preset.Name,
			Url = preset.IsGroup
				? null
				: NavigatorExt.AddQueryValueToUri(Navigator.GetLocalUrl(), TfConstants.PresetIdQueryName,
					preset.Id.ToString()),
			Color = preset.Color,
			IconExpanded = TfConstants.GetIcon(preset.Icon),
			IconCollapsed = TfConstants.GetIcon(preset.Icon),
		};

		foreach (var child in preset.Presets)
		{
			item.Items.Add(_getMenuItem(child));
		}

		return item;
	}

	private void _setSelection(TfMenuItem menuItem, Guid? currrentPresetId)
	{
		menuItem.Selected = currrentPresetId is not null
		                    && menuItem.IdTree.Contains(currrentPresetId.Value.ToString());

		foreach (var child in menuItem.Items)
		{
			_setSelection(child, currrentPresetId);
		}
	}
}