namespace WebVella.Tefter.UI.Components;

public partial class TucAdminTemplatesPageContent : TfBaseComponent, IAsyncDisposable
{
	private bool _isLoading = false;
	private List<TfTemplate> _items = new();
	private List<TfTemplateResultType> _toggledTypes = new();
	private IAsyncDisposable _templateEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _templateEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_templateEventSubscriber = await TfEventBus.SubscribeAsync<TfTemplateEventPayload>(
			handler: On_TemplateEventAsync,
			matchKey: (_) => true);
	}

	private async Task On_TemplateEventAsync(string? key, TfTemplateEventPayload? payload)
	{
		if(payload is null) return;
		if(key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}		

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
			{
				await _init(TfAuthLayout.GetState().NavigationState);
			}
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_toggledTypes = Navigator.GetListEnumFromQuery<TfTemplateResultType>(TfConstants.TabQueryName)
			                ?? Enum.GetValues<TfTemplateResultType>().ToList();
			_items = TfService.GetTemplates(navState.Search, _toggledTypes);
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucTemplateManageDialog>(
			new TfTemplate(),
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }) { }
	}

	private async Task _toggleType(TfTemplateResultType type)
	{
		var newList = _toggledTypes.ToList();
		if(newList.Contains(type))
			newList.Remove(type);
		else
			newList.Add(type);
		
		await Navigator.ApplyChangeToUrlQuery(TfConstants.TabQueryName,newList);

	}
}