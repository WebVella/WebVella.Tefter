namespace WebVella.Tefter.UI.Components;
public partial class TucHomeToolbar : TfBaseComponent, IDisposable
{
	[Parameter] public EventCallback<string> OnSearch { get; set; }
	[Parameter] public EventCallback<string> OnFilterToggle { get; set; }

	private async Task _searchChanged(string value) => await OnSearch.InvokeAsync(value);

	private async Task _toggleFilter(string propName) => await OnFilterToggle.InvokeAsync(propName);

	private TfNavigationState _navState = new();

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
	}
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}