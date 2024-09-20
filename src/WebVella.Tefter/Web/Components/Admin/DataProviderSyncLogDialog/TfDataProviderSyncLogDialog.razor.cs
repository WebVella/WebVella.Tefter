namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderSyncLogDialog.TfDataProviderSyncLogDialog", "WebVella.Tefter")]
public partial class TfDataProviderSyncLogDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderSyncTaskInfoLog>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucDataProviderSyncTaskInfoLog Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _title = "";
	private bool _isBusy = true;
	private List<TucDataProviderSyncTaskInfo> _items = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Content is null) throw new Exception("Content is null");

		switch (Content.Type)
		{
			case TucDataProviderSyncTaskInfoType.Info:
				_title = LOC("Synchronization informations");
				break;
			case TucDataProviderSyncTaskInfoType.Warning:
				_title = LOC("Synchronization warnings");
				break;
			case TucDataProviderSyncTaskInfoType.Error:
				_title = LOC("Synchronization errors");
				break;
		}

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			try
			{
				var result = UC.GetSynchronizationTaskLogRecords(Content.TaskId, Content.Type);
				ProcessServiceResponse(result);
				if (result.IsSuccess)
					_items = result.Value;
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

