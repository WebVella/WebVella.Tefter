namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataIdentityDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfDataIdentityUIService TfDataIdentityUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfDataIdentityUIService.DataIdentityCreated -= On_DataIdentityCreated;
		TfDataIdentityUIService.DataIdentityUpdated -= On_DataIdentityUpdated;
		TfDataIdentityUIService.DataIdentityDeleted -= On_DataIdentityDeleted;
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfDataIdentityUIService.DataIdentityCreated += On_DataIdentityCreated;
		TfDataIdentityUIService.DataIdentityUpdated += On_DataIdentityUpdated;
		TfDataIdentityUIService.DataIdentityDeleted += On_DataIdentityDeleted;
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}

	private async void On_DataIdentityCreated(object? caller, TfDataIdentity user)
	{
		await _init();
	}

	private async void On_DataIdentityUpdated(object? caller, TfDataIdentity user)
	{
		await _init();
	}

	private async void On_DataIdentityDeleted(object? caller, TfDataIdentity user)
	{
		await _init();
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
			_search = NavigatorExt.GetStringFromQuery(Navigator, TfConstants.AsideSearchQueryName, null);
			var roles = TfDataIdentityUIService.GetDataIdentities(_search).ToList();
			_items = new();
			foreach (var role in roles)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, role.DataIdentity),
					Description = role.IsSystem ? "system created" : "user created",
					Text = TfConverters.StringOverflow(role.DataIdentity, _stringLimit),
					Selected = navData.State.DataIdentityId == role.DataIdentity
				});
			}
		}
		finally
		{
			UriInitialized = navData.Uri;
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}