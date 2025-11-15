namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewSettingsSelector : TfBaseComponent
{
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataset SpaceData { get; set; } = null!;
	[Parameter] public TfUser User { get; set; } = null!;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;

	private bool _open = false;
	private bool _isFilteredOrSorted
	{
		get
		{
			var navState = TfAuthLayout.GetState().NavigationState;
			if (!String.IsNullOrWhiteSpace(navState.Search)) return true;
			if (navState.Filters is not null && navState.Filters.Count > 0) return true;
			if (navState.Sorts is not null && navState.Sorts.Count > 0) return true;
			return false;
		}
	}	
	
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}	
	private async Task _manageColorRulesHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColorRulesDialog>(
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
	
	private async Task _createPresetHandler()
	{
		var context = new TfPresetFilterManagementContext
		{
			Item = null,
			Parents = _getParents().ToList(),
			DateSet = SpaceData,
			SpaceView = SpaceView
		};
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
	
	private IEnumerable<TfSpaceViewPreset> _getParents()
	{
		var parents = new List<TfSpaceViewPreset>();
		foreach (var item in SpaceView.Presets)
		{
			_fillParents(parents, item);
		}
		return parents.AsEnumerable();
	}	
	private void _fillParents(List<TfSpaceViewPreset> parents, TfSpaceViewPreset current)
	{
		if (current.IsGroup) parents.Add(current);
		foreach (var item in current.Presets) _fillParents(parents, item);
	}	
}