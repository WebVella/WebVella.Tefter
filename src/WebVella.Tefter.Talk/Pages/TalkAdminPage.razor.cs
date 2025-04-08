namespace WebVella.Tefter.Talk.Pages;

public partial class TalkAdminPage : TfBaseComponent, ITucAuxDataUseComponent, ITfRegionComponent<TfAdminPageScreenRegion>
{
	public Guid Id { get; init; }
	public int PositionRank { get; init; }
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init; }
    public List<TfScreenRegionScope> Scopes { get; init; }
	[Parameter] 
	public TfAdminPageScreenRegion Context { get; init; }

	public TalkAdminPage() : base()
	{
		var componentId = new Guid("15f22367-7c8d-4971-9950-d7b1ff51f678");
		Id = componentId;
		PositionRank = 90;
		Name = "Talk Channels";
		Description = "";
		FluentIconName = "CommentMultiple";
		Scopes = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,componentId)
		};
	}

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