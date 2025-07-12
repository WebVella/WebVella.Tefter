namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfTemplateUIService TfTemplateUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfTemplateUIService.TemplateCreated -= On_TemplateCreated;
		TfTemplateUIService.TemplateUpdated -= On_TemplateUpdated;
		TfTemplateUIService.TemplateDeleted -= On_TemplateDeleted;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfTemplateUIService.TemplateCreated += On_TemplateCreated;
		TfTemplateUIService.TemplateUpdated += On_TemplateUpdated;
		TfTemplateUIService.TemplateDeleted += On_TemplateDeleted;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_TemplateCreated(object? caller, TfTemplate args)
	{
		await _init();
	}

	private async void On_TemplateUpdated(object? caller, TfTemplate args)
	{
		await _init();
	}

	private async void On_TemplateDeleted(object? caller, TfTemplate args)
	{
		await _init();
	}


	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationState(Navigator);

		try
		{
			_search = navState.SearchAside;
			var items = TfTemplateUIService.GetTemplates(_search,navState.TemplateResultType).ToList();

			_items = new();
			foreach (var item in items)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminTemplatesTemplatePageUrl, (int)item.ResultType, item.Id),
					Description = item.ResultType.ToDescriptionString(),
					Text = TfConverters.StringOverflow(item.Name, _stringLimit),
					Selected = navState.TemplateId == item.Id
				});
			}
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}