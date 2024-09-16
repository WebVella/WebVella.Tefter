namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.DataProviderSyncLogDialog.TfDataProviderSyncLogDialog","WebVella.Tefter")]
public partial class TfDataProviderSyncLogDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderSyncTaskInfoLog>
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] private IState<TfState> TfState { get; set; }
	[Parameter] public TucDataProviderSyncTaskInfoLog Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	PaginationState _pagination = new PaginationState { ItemsPerPage = 10 };

	private string _title = "";

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		UC.IsBusy = true;
		_pagination.ItemsPerPage = UC.TaskSyncLogPageSize;
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
			await UC.SetSynchronizationTaskLogRecords(Content.TaskId, Content.Type);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}



}

