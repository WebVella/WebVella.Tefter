namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataIdentityDetailsContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfDataIdentityUIService TfDataIdentityUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private TfDataIdentity? _identity = null;
	public bool _submitting = false;

	public void Dispose()
	{
		TfDataIdentityUIService.DataIdentityUpdated -= On_DataIdentityUpdated;
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfDataIdentityUIService.DataIdentityUpdated += On_DataIdentityUpdated;
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}

	private async void On_DataIdentityUpdated(object? caller, TfDataIdentity args)
	{
		await _init(identity: args);
	}

	private async void On_NavigationDataChanged(object? caller, TfSpaceNavigationData args)
	{
		if (UriInitialized != args.Uri)
			await _init(navData: args);
	}

	private async Task _init(TfSpaceNavigationData? navData = null, TfDataIdentity? identity = null)
	{
		if (navData is null)
			navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);
		try
		{
			if (identity is not null && identity.DataIdentity == _identity?.DataIdentity)
			{
				_identity = identity;
			}
			else
			{
				if (navData.State.DataIdentityId is not null)
					_identity = TfDataIdentityUIService.GetDataIdentity(navData.State.DataIdentityId);
			}
		}
		finally
		{
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editIdentity()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataIdentityManageDialog>(
		_identity,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}

	private async Task _deleteIdentity()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this identity deleted?")))
			return;
		try
		{
			TfDataIdentityUIService.DeleteDataIdentity(_identity.DataIdentity);
			ToastService.ShowSuccess(LOC("Data identity removed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

}