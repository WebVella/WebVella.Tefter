namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderJoinedData.TfAdminDataProviderJoinedData", "WebVella.Tefter")]
public partial class TfAdminDataProviderJoinedData : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private List<TucDataProvider> _joinedProviders = new();
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (TfAppState.Value.AdminDataProvider is null) return;
		_joinedProviders = await UC.GetDataProviderJoinedProvidersAsync(TfAppState.Value.AdminDataProvider.Id);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
		}
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			if (TfAppState.Value.AdminDataProvider is not null)
			{
				_joinedProviders = await UC.GetDataProviderJoinedProvidersAsync(TfAppState.Value.AdminDataProvider.Id);
				await InvokeAsync(StateHasChanged);
			}
		});
	}

	private async Task _addKey()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderKeyManageDialog>(
		new TucDataProviderJoinKey(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Key successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = record }));
		}
	}

	private async Task _editKey(TucDataProviderJoinKey key)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderKeyManageDialog>(
				key,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			ToastService.ShowSuccess(LOC("The key was successfully updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = (TucDataProvider)result.Data }));
		}
	}

	private async Task _deleteKey(TucDataProviderJoinKey key)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this key deleted?")))
			return;
		try
		{
			var provider = UC.DeleteDataProviderJoinKey(key.Id);
			ToastService.ShowSuccess(LOC("The key is successfully deleted!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = (TucDataProvider)provider }));

		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}

}