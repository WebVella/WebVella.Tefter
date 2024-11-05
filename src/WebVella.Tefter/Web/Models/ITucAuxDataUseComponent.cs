namespace WebVella.Tefter.Web.Models;

public interface ITucAuxDataUseComponent
{
	Task OnAppStateInit(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState);
}
