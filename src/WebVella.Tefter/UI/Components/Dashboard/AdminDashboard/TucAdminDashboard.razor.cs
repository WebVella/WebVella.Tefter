
namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.UI.Components.Admin.AdminDashboard.TucAdminDashboard", "WebVella.Tefter")]
public partial class TucAdminDashboard : TfBaseComponent
{
	[Inject] protected ITfDashboardUIService TfGeneralUIService { get; set; } = default!;

	private TfAdminDashboardData? _data = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_data = await TfGeneralUIService.GetAdminDashboardData();
	}
}