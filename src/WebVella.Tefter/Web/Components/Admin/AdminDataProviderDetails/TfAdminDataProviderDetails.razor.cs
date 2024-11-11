﻿
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderDetails.TfAdminDataProviderDetails","WebVella.Tefter")]
public partial class TfAdminDataProviderDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = TfComponentMode.Read;
		dict["Value"] = TfAppState.Value.AdminDataProvider?.SettingsJson;
		return dict;
	}

}