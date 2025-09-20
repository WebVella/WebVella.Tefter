namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProvidersPageContent : TfBaseComponent
{
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var dataProviders = TfDataProviderUIService.GetDataProviders();
		if (dataProviders.Count > 0)
		{
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataProviderDetailsPageUrl, dataProviders[0].Id));
		}
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDialog>(
		new TfDataProvider(),
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
			var provider = (TfDataProvider)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataProviderDetailsPageUrl, provider.Id));
		}
	}
}