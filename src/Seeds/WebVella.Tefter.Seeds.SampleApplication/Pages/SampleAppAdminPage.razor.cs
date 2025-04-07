namespace WebVella.Tefter.Seeds.SampleApplication.Pages;

public partial class SampleAppAdminPage : TfBaseComponent, ITucAuxDataUseComponent, ITfRegionComponent<TfAdminPageComponentContext>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }

	public Guid Id { get; init; } = new Guid("afc65ec6-5e97-4149-aa0a-b39eb4c6561a");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Sample App";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "Album";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){
		new TfRegionComponentScope(null,new Guid("0153dbe3-ba62-4df4-8f9d-d0ed889c1444"))
	};
	[Parameter]
	public TfAdminPageComponentContext Context { get; init; }


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