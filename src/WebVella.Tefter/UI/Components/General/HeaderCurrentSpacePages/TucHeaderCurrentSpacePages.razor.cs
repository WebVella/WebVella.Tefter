namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderCurrentSpacePages : TfBaseComponent
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	private List<TfMenuItem> _menu = new();
	private bool _isLoading = true;

	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		try
		{
			_menu = (await TfNavigationUIService.GetNavigationMenu(Navigator)).Menu;
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}