namespace WebVella.Tefter.Talk.Pages;
[TfScreenRegionComponent(TfScreenRegion.AdminPages, 1, "Talk Administration", "talk-dashboard")]
public partial class TalkAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent
{
    public Task OnSpaceViewStateInited(IServiceProvider serviceProvider,TucUser currentUser,
        TfRouteState routeState, TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var talkService = serviceProvider.GetRequiredService<ITalkService>();        
        var sharedColumnsManager = serviceProvider.GetRequiredService<ITfSharedColumnsManager>();        
        var srvResult = talkService.GetChannels();
        if(srvResult.IsSuccess) 
            newAuxDataState.Data[Constants.TALK_APP_CHANNEL_LIST_DATA_KEY] = srvResult.Value.OrderBy(x=> x.Name).ToList();
        else 
            newAuxDataState.Data[Constants.TALK_APP_CHANNEL_LIST_DATA_KEY] = new List<TalkChannel>();

        var sharedColumnsResult = sharedColumnsManager.GetSharedColumns();
        if(sharedColumnsResult.IsSuccess) 
            newAuxDataState.Data[Constants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = sharedColumnsResult.Value.OrderBy(x=> x.DbName).ToList();
        else
            newAuxDataState.Data[Constants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();

        return Task.CompletedTask;
    }
}