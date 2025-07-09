namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderCurrentSpacePages : TfBaseComponent
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	private List<TfMenuItem> _menu = new();
	private bool _isLoading = true;

	public void Dispose()
	{
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}

	private async void On_NavigationDataChanged(object? caller, TfSpaceNavigationData args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async Task _init(TfSpaceNavigationData? navData = null)
	{
		if (navData is null)
			navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);
		try
		{
			_menu = navData.Menu;
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}