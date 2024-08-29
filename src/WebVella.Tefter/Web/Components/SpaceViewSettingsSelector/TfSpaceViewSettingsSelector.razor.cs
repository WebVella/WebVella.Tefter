namespace WebVella.Tefter.Web.Components.SpaceViewSettingsSelector;
public partial class TfSpaceViewSettingsSelector : TfBaseComponent
{
    [Inject] protected IState<SpaceState> SpaceState { get; set; }
	private bool _open = false;
	private bool _selectorLoading = false;

	private void _init()
	{
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			_init();

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}