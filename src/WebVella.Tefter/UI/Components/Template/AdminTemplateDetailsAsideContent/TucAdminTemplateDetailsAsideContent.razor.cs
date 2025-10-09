namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider?.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.TemplateCreatedEvent += On_TemplateCreated;
		TfEventProvider.TemplateUpdatedEvent += On_TemplateUpdated;
		TfEventProvider.TemplateDeletedEvent += On_TemplateDeleted;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_TemplateCreated(TfTemplateCreatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_TemplateUpdated(TfTemplateUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_TemplateDeleted(TfTemplateDeletedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
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