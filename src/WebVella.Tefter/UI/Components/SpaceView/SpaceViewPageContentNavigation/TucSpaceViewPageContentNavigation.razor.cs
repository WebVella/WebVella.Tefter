namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentNavigation : TfBaseComponent
{
	// Dependency Injection
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;

	private List<TfMenuItem> _menu = new();
	public TfNavigationState _navState = default!;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init();
	}

	private async Task _init()
	{
		_navState = TfAuthLayout.NavigationState;

		try
		{
			_menu = new();
			Guid? currentPresetId = Navigator.GetGuidFromQuery(TfConstants.PresetIdQueryName);
			if (SpaceView.Presets.Count > 0)
			{
				_menu.Add(new TfMenuItem
				{
					Id = "tf-main",
					Text = "Main",
					Url = NavigatorExt.AddQueryValueToUri(Navigator.Uri, TfConstants.PresetIdQueryName, null),
					Selected = currentPresetId is null
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
			await InvokeAsync(StateHasChanged);
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