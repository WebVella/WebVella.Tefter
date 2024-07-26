namespace WebVella.Tefter.Web.Components.DataProviderSyncLogDialog;
public partial class TfDataProviderSyncLogDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderSyncLog>
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] private IState<DataProviderAdminState> DataProviderDetailsState { get; set; }
	[Parameter] public TucDataProviderSyncLog Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _title = "";


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		UC.IsBusy = true;
		if (Content is null) throw new Exception("Content is null");

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _loadDataAsync()
	{

	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	

}

