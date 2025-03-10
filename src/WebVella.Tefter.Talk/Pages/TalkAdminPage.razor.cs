namespace WebVella.Tefter.Talk.Pages;

public partial class TalkAdminPage : TfBaseComponent, ITucAuxDataUseComponent, ITfRegionComponent<TfAdminPageComponentContext>
{
	public Guid Id { get; init; } = new Guid("15f22367-7c8d-4971-9950-d7b1ff51f678"); 
	public int PositionRank { get; init; } = 90;
	public string Name { get; init;} = "Talk Channels";
	public string Description { get; init;} = "";
	public string FluentIconName { get; init; } =  "CommentMultiple";
    public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){
        new TfRegionComponentScope(null,new Guid("15f22367-7c8d-4971-9950-d7b1ff51f678"))
    };
	[Parameter] 
	public TfAdminPageComponentContext Context { get; init; }

	public Task OnAppStateInit(IServiceProvider serviceProvider,TucUser currentUser,
        TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var talkService = serviceProvider.GetRequiredService<ITalkService>();        
        var tfService = serviceProvider.GetRequiredService<ITfService>();
		try
		{
			var channels = talkService.GetChannels();
			newAuxDataState.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = channels.OrderBy(x => x.Name).ToList();
		}
		catch
		{
			newAuxDataState.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = new List<TalkChannel>();
		}

		try
		{
			var sharedColumns = tfService.GetSharedColumns();
			newAuxDataState.Data[TalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = sharedColumns.OrderBy(x => x.DbName).ToList();
		}
		catch
		{
			newAuxDataState.Data[TalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();
		}

        return Task.CompletedTask;
    }
}