﻿namespace WebVella.Tefter.Web.Components;
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

	private async void On_AppChanged(SetAppStateAction action)
	{
		await InvokeAsync(async () =>
		{
			if (TfAppState.Value.AdminDataProvider is not null)
			{
				_joinedProviders = await UC.GetDataProviderJoinedProvidersAsync(TfAppState.Value.AdminDataProvider.Id);
				StateHasChanged();
			}
		});
	}

	private async Task _implementDataIdentity()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderIdentityManageDialog>(
		new TucDataProviderIdentity(),
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
			ToastService.ShowSuccess(LOC("Data Identity successfully implemented!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = record }));
		}
	}

	private async Task _editIdentity(TucDataProviderIdentity identity)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderIdentityManageDialog>(
				identity,
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
			ToastService.ShowSuccess(LOC("Identity implementation was successfully updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = (TucDataProvider)result.Data }));
		}
	}

	private async Task _deleteIdentity(TucDataProviderIdentity key)
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

	private string _showCommonIdentities(TucDataProvider subProvider)
	{
		var commonIdentities = TfAppState.Value.AdminDataProvider.Identities
							.Select(x => x.Name)
							.Intersect(subProvider.Identities.Select(x => x.Name)).ToList();
		if (commonIdentities.Count == 0) return "n/a";
		return String.Join(", ", commonIdentities);
	}

}