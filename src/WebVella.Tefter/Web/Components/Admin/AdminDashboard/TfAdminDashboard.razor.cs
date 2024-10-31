namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDashboard.TfAdminDashboard","WebVella.Tefter")]
public partial class TfAdminDashboard : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

}