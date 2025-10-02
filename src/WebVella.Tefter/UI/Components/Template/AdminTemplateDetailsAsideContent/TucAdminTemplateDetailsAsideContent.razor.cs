namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider.TemplateCreatedEvent -= On_TemplateCreated;
		TfEventProvider.TemplateUpdatedEvent -= On_TemplateUpdated;
		TfEventProvider.TemplateDeletedEvent -= On_TemplateDeleted;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.TemplateCreatedEvent += On_TemplateCreated;
		TfEventProvider.TemplateUpdatedEvent += On_TemplateUpdated;
		TfEventProvider.TemplateDeletedEvent += On_TemplateDeleted;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_TemplateCreated(TfTemplateCreatedEvent args)
	{
		await _init(TfAuthLayout.NavigationState);
	}

	private async void On_TemplateUpdated(TfTemplateUpdatedEvent args)
	{
		await _init(TfAuthLayout.NavigationState);
	}

	private async void On_TemplateDeleted(TfTemplateDeletedEvent args)
	{
		await _init(TfAuthLayout.NavigationState);
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
			var items = TfService.GetTemplates(_search,navState.TemplateResultType).ToList();

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