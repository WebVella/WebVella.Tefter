namespace WebVella.Tefter.UI.Components;
public partial class TucPageTopbarBreadcrumb : TfBaseComponent,IDisposable
{
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}
	protected override void OnInitialized()
	{
		_init();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if(firstRender)
			Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private void _init()
	{

		_items = TfAuthLayout.GetState().Breadcrumb;
		_items[0].Url = "#";
		_items[0].OnClick = EventCallback.Factory.Create(this, async () => await _findSpace());
		
	}

	private async Task _findSpace()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceFinderDialog>(
			TfAuthLayout.GetState().User,
			new ()
			{
				PreventDismissOnOverlayClick = false,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}		
}