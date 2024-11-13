namespace WebVella.Tefter.Talk.Pages;

public partial class TalkAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
    public Guid Id { get { return new Guid("15f22367-7c8d-4971-9950-d7b1ff51f678"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 10; } }
	public string Name { get { return "Talk Channels"; } }
	public string FluentIconName { get { return "CommentMultiple"; } }

	public Task OnAppStateInit(IServiceProvider serviceProvider,TucUser currentUser,
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