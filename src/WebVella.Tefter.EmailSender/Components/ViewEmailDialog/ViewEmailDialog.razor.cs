namespace WebVella.Tefter.EmailSender.Components;
public partial class ViewEmailDialog : TfBaseComponent, IDialogContentComponent<EmailMessage>
{
	[Inject] public IEmailService TalkService { get; set; }
	[Parameter] public EmailMessage Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private Dictionary<Guid, TfSpace> _spaceDict = new();
	private Dictionary<Guid, TfDataset> _datasetDict = new();
	
	private string _error = string.Empty;
	private string _activeTab = "text";

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			_spaceDict = TfService.GetSpacesList().ToDictionary(x => x.Id);
			_datasetDict = TfService.GetDatasets().ToDictionary(x => x.Id);
		}
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
