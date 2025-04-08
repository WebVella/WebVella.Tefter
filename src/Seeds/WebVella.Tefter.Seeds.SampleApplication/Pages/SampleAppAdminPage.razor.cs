namespace WebVella.Tefter.Seeds.SampleApplication.Pages;

public partial class SampleAppAdminPage : TfBaseComponent, ITucAuxDataUseComponent, ITfRegionComponent<TfAdminPageScreenRegion>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }

	//Id of the component
	public Guid Id { get; init; } 
	//Position ranking
	public int PositionRank { get; init; }
	//Human readable name of the component
	public string Name { get; init; }
	//Human readable name of the component
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfScreenRegionScope> Scopes { get; init; }
	[Parameter]
	public TfAdminPageScreenRegion Context { get; init; }

	public SampleAppAdminPage() : base()
	{
		var componentId = new Guid("afc65ec6-5e97-4149-aa0a-b39eb4c6561a");
		Id = componentId;
		PositionRank = 1000;
		Name = "Sample App";
		Description = "";
		FluentIconName = "Album";
		Scopes = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,componentId)
		};
	}

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