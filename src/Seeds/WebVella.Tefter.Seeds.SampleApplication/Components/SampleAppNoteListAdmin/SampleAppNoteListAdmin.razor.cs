namespace WebVella.Tefter.Seeds.SampleApplication.Components;
public partial class SampleAppNoteListAdmin : TfBaseComponent
{
	[Inject] public ISampleAppService SampleAppService { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
}