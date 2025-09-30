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
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			_navState = TfAuthLayout.NavigationState;
		else 
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