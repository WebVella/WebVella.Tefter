﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderDetailsActions.TfAdminDataProviderDetailsActions", "WebVella.Tefter")]
public partial class TfAdminDataProviderDetailsActions : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private bool _isDeleting = false;
	private async Task _editProvider()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderManageDialog>(TfAppState.Value.AdminDataProvider,
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
			
			ToastService.ShowSuccess(LOC("Provider successfully updated!"));
			var item = (TucDataProvider)result.Data;
			var newState = TfAppState.Value with { AdminDataProvider = item };
			var index = TfAppState.Value.AdminDataProviders.FindIndex(x => x.Id == item.Id);
			if (index > -1)
			{
				var items = TfAppState.Value.AdminDataProviders.ToList();
				items[index] = item;
				newState = newState with { AdminDataProviders = items };
			}
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = item }));
		}
	}

	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderColumnManageDialog>(
		new TucDataProviderColumn(),
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
			ToastService.ShowSuccess(LOC("Column successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = record }));
		}
	}

	private async Task _importFromSource()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderImportSchemaDialog>(
				TfAppState.Value.AdminDataProvider,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthFullScreen,
					TrapFocus = false,
					
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Column successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = record }));
		}
	}

	private async Task _deleteProvider()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this data provider deleted?") + "\r\n" + LOC("Will proceeed only if there are not existing columns attached")))
			return;

		if (TfAppState.Value is null || TfAppState.Value.AdminDataProvider is null) return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			await UC.DeleteDataProviderAsync(TfAppState.Value.AdminDataProvider.Id);
			Navigator.NavigateTo(TfConstants.AdminDataProvidersPageUrl, true);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isDeleting = false;
			await InvokeAsync(StateHasChanged);
		}

	}

}