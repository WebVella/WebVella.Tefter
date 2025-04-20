
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDashboard.TfAdminDashboard", "WebVella.Tefter")]
public partial class TfAdminDashboard : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private bool _providerInfoLoading = true;

	private ReadOnlyCollection<TucDataProviderInfo> _providersInfo = null;
	private List<TucDataProviderInfo> _syncInfo = null;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (firstRender)
		{
			await _loadProviderInfo();
			await InvokeAsync(StateHasChanged);
		}
	}


	private async Task _loadProviderInfo()
	{
		_providersInfo = await UC.GetDataProvidersInfoAsync();
		_syncInfo = _providersInfo.Where(x=> x.NextSyncOn is not null).OrderBy(x => x.NextSyncOn).Take(5).ToList();
		_providerInfoLoading = false;
	}

}