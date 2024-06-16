namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewShareSelector : TfBaseComponent
{
	private bool _open = false;
	private bool _selectorLoading = false;
	private int _rowsCount = 2;

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