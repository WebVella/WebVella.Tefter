namespace WebVella.Tefter.Talk.Pages;
[TfScreenRegionComponent(TfScreenRegion.AdminPages, 1, "Talk Administration", "talk-dashboard")]
public partial class TalkAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent
{
    [Inject] internal ITalkService TalkService { get; set; }
    public Task OnSpaceViewStateInited(IIdentityManager identityManager,
        ITfDataProviderManager dataProviderManager, ITfSharedColumnsManager sharedColumnsManager,
        IDataManager dataManager, ITfSpaceManager spaceManager, TucUser currentUser,
        TfRouteState routeState, TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var srvResult = TalkService.GetChannels();
        if(srvResult.IsSuccess) newAuxDataState.Data[Constants.TALK_APP_CHANNEL_LIST_DATA_KEY] = srvResult.Value;
        else newAuxDataState.Data[Constants.TALK_APP_CHANNEL_LIST_DATA_KEY] = new List<TalkChannel>();

        return Task.CompletedTask;
    }
}