using ITfEventBus = WebVella.Tefter.UI.EventsBus.ITfEventBus;

namespace WebVella.Tefter.UI.Components;

public partial class TucAdminTemplatesPageContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBus TfEventBus { get; set; } = null!;
	private bool _isLoading = false;
	private List<TfTemplate> _items = new();
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
			handler: On_TemplateEventAsync);
	}

	private async Task On_TemplateEventAsync(string? key, TfTemplateEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);

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
			_items = TfService.GetTemplates(navState.Search).ToList();
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
}