using WebVella.Tefter.Api;

namespace WebVella.Tefter.Web.Models;

public interface ITucAuxDataUseComponent
{
	Task OnSpaceViewStateInited(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState);
}
