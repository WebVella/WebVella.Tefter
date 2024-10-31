namespace WebVella.Tefter.Talk.Pages;

public partial class TalkAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 1; } }
	public string Name { get { return "Talk Administration"; } }
	public string UrlSlug { get { return "talk-dashboard"; } }

	public Task OnSpaceViewStateInited(IServiceProvider serviceProvider,TucUser currentUser,
        TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var talkService = serviceProvider.GetRequiredService<ITalkService>();        
        var sharedColumnsManager = serviceProvider.GetRequiredService<ITfSharedColumnsManager>();        
        var srvResult = talkService.GetChannels();
        if(srvResult.IsSuccess) 
            newAuxDataState.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = srvResult.Value.OrderBy(x=> x.Name).ToList();
        else 
            newAuxDataState.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = new List<TalkChannel>();

        var sharedColumnsResult = sharedColumnsManager.GetSharedColumns();
        if(sharedColumnsResult.IsSuccess) 
            newAuxDataState.Data[TfTalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = sharedColumnsResult.Value.OrderBy(x=> x.DbName).ToList();
        else
            newAuxDataState.Data[TfTalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();

        return Task.CompletedTask;
    }
}