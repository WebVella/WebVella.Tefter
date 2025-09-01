namespace WebVella.Tefter.Talk.Addons;

public partial class TalkAdminPage : TfBaseComponent, ITfScreenRegionComponent<TfAdminPageScreenRegionContext>
{

	public const string ID = "15f22367-7c8d-4971-9950-d7b1ff51f678";
	public const string NAME = "Talk Channels";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "CommentMultiple";
	public const int POSITION_RANK = 90;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,new Guid(ID))
		};
	[Parameter]
	public TfAdminPageScreenRegionContext RegionContext { get; set; }

	//public Task OnAppStateInit(IServiceProvider serviceProvider, TucUser currentUser,
	//	TfAppState newAppState,
	//	TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	//{
	//	var talkService = serviceProvider.GetRequiredService<ITalkService>();
	//	var tfService = serviceProvider.GetRequiredService<ITfService>();
	//	try
	//	{
	//		var channels = talkService.GetChannels();
	//		newAuxDataState.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = channels.OrderBy(x => x.Name).ToList();
	//	}
	//	catch
	//	{
	//		newAuxDataState.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = new List<TalkChannel>();
	//	}

	//	try
	//	{
	//		var sharedColumns = tfService.GetSharedColumns();
	//		newAuxDataState.Data[TalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = sharedColumns.OrderBy(x => x.DbName).ToList();
	//	}
	//	catch
	//	{
	//		newAuxDataState.Data[TalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();
	//	}

	//	return Task.CompletedTask;
	//}
}