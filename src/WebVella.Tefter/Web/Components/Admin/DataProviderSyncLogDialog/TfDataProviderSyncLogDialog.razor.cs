namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderSyncLogDialog.TfDataProviderSyncLogDialog", "WebVella.Tefter")]
public partial class TfDataProviderSyncLogDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderSyncTask>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucDataProviderSyncTask Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _title = "";
	private bool _isBusy = true;
	private ReadOnlyCollection<TfDataProviderSychronizationLogEntry> _items = 
		new List<TfDataProviderSychronizationLogEntry>().AsReadOnly();
	private int _limit = 1000;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Synchronization informations");
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			try
			{
				_items = Content.SynchronizationLog.GetEntries();
				//_items = UC.GetSynchronizationTaskLogRecords(Content.TaskId);
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
			finally
			{
				_isBusy = false;
				await InvokeAsync(StateHasChanged);
			}
		}
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}



}

