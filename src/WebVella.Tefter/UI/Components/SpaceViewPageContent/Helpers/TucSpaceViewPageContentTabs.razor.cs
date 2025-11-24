namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContentTabs : TfBaseComponent, IAsyncDisposable
{
	// Dependency Injection
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfSpacePage SpacePage { get; set; } = null!;
	[Parameter] public TfDataset SpaceData { get; set; } = null!;

	private List<TfMenuItem> _menu = new();
	private TfNavigationState? _navState = null!;
	private Guid _initedViewId = Guid.Empty;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.SpaceViewUpdatedEvent -= On_SpaceViewUpdated;
		await TfEventProvider.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init();
	}

	protected override void OnParametersSet()
	{
		if (_initedViewId != SpaceView.Id)
			_init();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			Navigator.LocationChanged += On_NavigationStateChanged;
			TfEventProvider.SpaceViewUpdatedEvent += On_SpaceViewUpdated;
		}
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
			{
				_init();
				await InvokeAsync(StateHasChanged);
			}
		});
	}

	private async Task On_SpaceViewUpdated(TfSpaceViewUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			_init(newView: args.Payload);
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _init(TfSpaceView? newView = null)
	{
		_navState = TfAuthLayout.GetState().NavigationState;
		TfSpaceView spaceView = newView ?? SpaceView;
		try
		{
			_menu = new();
			Guid? currentPresetId = Navigator.GetGuidFromQuery(TfConstants.PresetIdQueryName);
			Icon? mainIcon = String.IsNullOrWhiteSpace(spaceView.Settings.MainTabFluentIcon)
				? null
				: TfConstants.GetIcon(spaceView.Settings.MainTabFluentIcon);
			string mainLabel = spaceView.Settings.MainTabLabel;
			if (String.IsNullOrWhiteSpace(mainLabel)
			    && mainIcon is null)
				mainLabel = LOC("Main");

			if (spaceView.Presets.Count > 0)
			{
				_menu.Add(new TfMenuItem
				{
					Id = "tf-main",
					Text = mainLabel,
					Url = NavigatorExt.AddQueryValueToUri(Navigator.GetLocalUrl(), TfConstants.PresetIdQueryName,
						null),
					Selected = currentPresetId is null,
					IconCollapsed = mainIcon,
					IconExpanded = mainIcon,
					Color = spaceView.Settings.MainTabColor,
					Actions = new List<TfMenuItem>()
					{
						new TfMenuItem()
						{
							Text = "Manage Preset",
							IconCollapsed = TfConstants.GetIcon("Settings"),
							IconExpanded = TfConstants.GetIcon("Settings"),
							OnClick = EventCallback.Factory.Create(this, async () => await _manageMainHandler())
						}
					}
				});

				foreach (var prItem in spaceView.Presets)
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
			_initedViewId = spaceView.Id;
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
			IconCollapsed = TfConstants.GetIcon(preset.Icon),
			IconExpanded = TfConstants.GetIcon(preset.Icon)
		};
		if (TfAuthLayout.GetState().User.IsAdmin)
		{
			item.Actions.Add(new TfMenuItem()
			{
				Text = "Manage Preset",
				IconCollapsed = TfConstants.GetIcon("Settings"),
				IconExpanded = TfConstants.GetIcon("Settings"),
				OnClick = EventCallback.Factory.Create(this, async () => await _managePresetHandler(preset))
			});
			item.Actions.Add(new TfMenuItem() { IsDivider = true });
			item.Actions.Add(new TfMenuItem()
			{
				Text = preset.ParentId is null ? "Move left" : "Move up",
				IconCollapsed =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowLeft") : TfConstants.GetIcon("ArrowUp"),
				IconExpanded =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowLeft") : TfConstants.GetIcon("ArrowUp"),
				OnClick = EventCallback.Factory.Create(this, async () => await _movePresetHandler(preset, true))
			});
			item.Actions.Add(new TfMenuItem()
			{
				Text = preset.ParentId is null ? "Move right" : "Move down",
				IconCollapsed =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowRight") : TfConstants.GetIcon("ArrowDown"),
				IconExpanded =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowRight") : TfConstants.GetIcon("ArrowDown"),
				OnClick = EventCallback.Factory.Create(this, async () => await _movePresetHandler(preset, false))
			});
			item.Actions.Add(new TfMenuItem() { IsDivider = true });
			item.Actions.Add(new TfMenuItem()
			{
				Text = "Delete Preset",
				IconCollapsed = TfConstants.GetIcon("Delete")!.WithColor(Color.Error),
				IconExpanded = TfConstants.GetIcon("Settings")!.WithColor(Color.Error),
				OnClick = EventCallback.Factory.Create(this, async () => await _deletePresetHandler(preset))
			});
		}

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

	private async Task _manageMainHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageMainTabDialog>(
			SpaceView,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	private async Task _managePresetHandler(TfSpaceViewPreset preset)
	{
		var context = new TfPresetFilterManagementContext { Item = preset, DateSet = SpaceData, SpaceView = SpaceView };
		var dialog = await DialogService.ShowDialogAsync<TucPresetFilterManageDialog>(
			context,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}


	private async Task _deletePresetHandler(TfSpaceViewPreset preset)
	{
		try
		{
			await TfService.RemoveSpaceViewPreset(SpaceView.Id, preset.Id);
			ToastService.ShowSuccess(LOC("Preset successfully removed!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _movePresetHandler(TfSpaceViewPreset preset, bool isUp)
	{
		try
		{
			await TfService.MoveSpaceViewPreset(SpaceView.Id, preset.Id, isUp);
			ToastService.ShowSuccess(LOC("Preset successfully moved!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}
}