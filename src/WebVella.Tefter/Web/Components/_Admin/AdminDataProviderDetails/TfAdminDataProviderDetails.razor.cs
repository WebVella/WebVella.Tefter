
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderDetails.TfAdminDataProviderDetails","WebVella.Tefter")]
public partial class TfAdminDataProviderDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = ComponentDisplayMode.ReadOnly;
		dict["Value"] = TfAppState.Value?.AdminManagedDataProvider?.SettingsJson;
		return dict;
	}

}