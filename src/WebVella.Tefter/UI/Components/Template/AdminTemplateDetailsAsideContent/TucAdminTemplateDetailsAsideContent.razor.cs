namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfUIService.TemplateCreated -= On_TemplateCreated;
		TfUIService.TemplateUpdated -= On_TemplateUpdated;
		TfUIService.TemplateDeleted -= On_TemplateDeleted;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(Navigator.GetRouteState());
		TfUIService.TemplateCreated += On_TemplateCreated;
		TfUIService.TemplateUpdated += On_TemplateUpdated;
		TfUIService.TemplateDeleted += On_TemplateDeleted;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_TemplateCreated(object? caller, TfTemplate args)
	{
		await _init(Navigator.GetRouteState());
	}

	private async void On_TemplateUpdated(object? caller, TfTemplate args)
	{
		await _init(Navigator.GetRouteState());
	}

	private async void On_TemplateDeleted(object? caller, TfTemplate args)
	{
		await _init(Navigator.GetRouteState());
	}


	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var items = TfUIService.GetTemplates(_search,navState.TemplateResultType).ToList();

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