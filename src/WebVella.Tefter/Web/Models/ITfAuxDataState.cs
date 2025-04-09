namespace WebVella.Tefter.Web.Models;

public interface ITfAuxDataState
{
	Task OnAppStateInit(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState);
}
