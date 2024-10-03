using WebVella.Tefter.Api;

namespace WebVella.Tefter.Web.Models;

public interface ITucAuxDataUseComponent
{
	Task OnSpaceViewStateInited(
		IIdentityManager identityManager,
		ITfDataProviderManager dataProviderManager,
		ITfSharedColumnsManager sharedColumnsManager,
		IDataManager dataManager,
		ITfSpaceManager spaceManager,
		TucUser currentUser,
		TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState);
}
