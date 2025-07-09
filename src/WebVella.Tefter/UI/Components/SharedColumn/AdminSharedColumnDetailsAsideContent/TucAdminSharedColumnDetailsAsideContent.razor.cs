namespace WebVella.Tefter.UI.Components;
public partial class TucAdminSharedColumnDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSharedColumnUIService TfSharedColumnUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfSharedColumnUIService.SharedColumnCreated -= On_SharedColumnCreated;
		TfSharedColumnUIService.SharedColumnUpdated -= On_SharedColumnUpdated;
		TfSharedColumnUIService.SharedColumnDeleted -= On_SharedColumnDeleted;
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSharedColumnUIService.SharedColumnCreated += On_SharedColumnCreated;
		TfSharedColumnUIService.SharedColumnUpdated += On_SharedColumnUpdated;
		TfSharedColumnUIService.SharedColumnDeleted += On_SharedColumnDeleted;
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}

	private async void On_SharedColumnCreated(object? caller, TfSharedColumn item)
	{
		await _init();
	}

	private async void On_SharedColumnUpdated(object? caller, TfSharedColumn item)
	{
		await _init();
	}

	private async void On_SharedColumnDeleted(object? caller, TfSharedColumn item)
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
		if (navData == null)
			navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);
		try
		{
			_search = navData.State.Search;
			var items = TfSharedColumnUIService.GetSharedColumns(_search).ToList();
			_items = new();
			foreach (var item in items)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminSharedColumnDetailsPageUrl, item.Id),
					Description = item.DataIdentity,
					Text = TfConverters.StringOverflow(item.DbName, _stringLimit),
					Selected = navData.State.SharedColumnId == item.Id
				});
			}
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}