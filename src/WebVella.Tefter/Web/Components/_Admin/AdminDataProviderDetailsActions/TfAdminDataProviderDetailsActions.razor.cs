namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderDetailsActions.TfAdminDataProviderDetailsActions","WebVella.Tefter")]
public partial class TfAdminDataProviderDetailsActions : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfState { get; set; }
	private bool _isDeleting = false;
	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing) Navigator.LocationChanged -= Navigator_LocationChanged;
		return base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			await UC.InitActionsMenu(e.Location);
			await InvokeAsync(StateHasChanged);
		});
	}


	private async Task _editProvider()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderManageDialog>(TfState.Value.Provider,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Provider successfully updated!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(component:this, provider: record));
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
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Column successfully created!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(component:this, provider: record));
		}
	}

	private async Task _addAuxColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderAuxColumnManageDialog>(
		new TucDataProviderAuxColumn(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Column successfully created!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(component:this, provider: record));
		}
	}

	private async Task _deleteProvider()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this data provider deleted?") + "\r\n" + LOC("Will proceeed only if there are not existing columns attached")))
			return;

		if (TfState.Value is null || TfState.Value.Provider is null) return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			var result = UC.DeleteDataProvider(TfState.Value.Provider.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				Navigator.NavigateTo(TfConstants.AdminDataProvidersPageUrl, true);
			}
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

	private async Task _addKey()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderKeyManageDialog>(
		new TucDataProviderSharedKey(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Key successfully created!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(component:this, provider:  record));
		}
	}
}