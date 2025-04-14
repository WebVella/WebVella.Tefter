namespace WebVella.Tefter.Seeds.SampleApplication.Pages;

public partial class SampleAppAdminPage : TfBaseComponent, ITfAuxDataState, ITfRegionComponent<TfAdminPageScreenRegionContext>
{
	public const string ID = "afc65ec6-5e97-4149-aa0a-b39eb4c6561a";
	public const string NAME = "Sample App";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "Album";
	public const int POSITION_RANK = 1000;

	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }

	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,new Guid(ID))
		};
	[Parameter]
	public TfAdminPageScreenRegionContext RegionContext { get; init; }

	public Task OnAppStateInit(IServiceProvider serviceProvider, TucUser currentUser,
		TfAppState newAppState,
		TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		var sampleAppService = serviceProvider.GetRequiredService<ISampleAppService>();
		try
		{
			var notes = sampleAppService.GetNotes();
			newAuxDataState.Data[SampleAppConstants.APP_NOTES_LIST_DATA_KEY] = notes;
		}
		catch
		{
			newAuxDataState.Data[SampleAppConstants.APP_NOTES_LIST_DATA_KEY] = new List<Note>();
		}

		return Task.CompletedTask;
	}
}