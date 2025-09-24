namespace WebVella.Tefter.UI.Components;

public partial class TucDatasetSortOrderDialog : TfBaseComponent, IDialogContentComponent<TfDataset?>
{
	[Inject] protected ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] protected ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] protected ITfDatasetUIService TfDatasetUIService { get; set; } = default!;
	[Parameter] public TfDataset? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;

	private TfDataset _dataset = new();
	private TfDataProvider _provider = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.DataProviderId == Guid.Empty) throw new Exception("DataProviderId is required");
		if (Content.Id == Guid.Empty) _isCreate = true;

		_provider = TfDataProviderUIService.GetDataProvider(Content.DataProviderId);
		if(_provider is null) throw new Exception("DataProviderId not found");

		_title = LOC("Manage primary sort order");
		_btnText = LOC("Save");
		_iconBtn = TfConstants.GetIcon("Save")!;
		_dataset = Content with { Id = Content.Id };

	}

	private async Task _onSortChanged(List<TfSort> sorts)
	{
		_dataset.SortOrders = sorts;
	}


	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TfDatasetUIService.UpdateDatasetSorts(_dataset.Id, _dataset.SortOrders);
			await Dialog.CloseAsync(_dataset);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}
}
