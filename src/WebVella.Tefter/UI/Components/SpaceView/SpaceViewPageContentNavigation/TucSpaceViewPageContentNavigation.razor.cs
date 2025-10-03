namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContentNavigation : TfBaseComponent
{
	// Dependency Injection
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfSpacePage SpacePage { get; set; } = null!;

	private List<TfMenuItem> _menu = new();
	private TfNavigationState? _navState = null!;
	private Guid _initedViewId = Guid.Empty;

	public void Dispose()
	{
		TfEventProvider.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	protected override async Task OnParametersSetAsync()
	{
		if (_initedViewId != SpaceView.Id)
			await _init(TfAuthLayout.NavigationState);
	}

	private async void On_NavigationStateChanged(TfNavigationStateChangedEvent args)
	{
		await InvokeAsync(async () =>
		{
			if (args.IsUserApplicable(TfAuthLayout.CurrentUser) && UriInitialized != args.Payload.Uri)
			{
				await _init(args.Payload);
				await InvokeAsync(StateHasChanged);
			}
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
			_menu = new();
			Guid? currentPresetId = Navigator.GetGuidFromQuery(TfConstants.PresetIdQueryName);
			var mainIcon = String.IsNullOrWhiteSpace(SpaceView.Settings.MainTabFluentIcon)
				? TfConstants.GetIcon(SpacePage.FluentIconName)
				: TfConstants.GetIcon(SpaceView.Settings.MainTabFluentIcon);
			if (SpaceView.Presets.Count > 0)
			{
				_menu.Add(new TfMenuItem
				{
					Id = "tf-main",
					Text = String.IsNullOrWhiteSpace(SpaceView.Settings.MainTabLabel) ?  "Main" : SpaceView.Settings.MainTabLabel,
					Url = NavigatorExt.AddQueryValueToUri(Navigator.Uri, TfConstants.PresetIdQueryName, null),
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
			UriInitialized = _navState.Uri;
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
				: NavigatorExt.AddQueryValueToUri(Navigator.Uri, TfConstants.PresetIdQueryName, preset.Id.ToString()),
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