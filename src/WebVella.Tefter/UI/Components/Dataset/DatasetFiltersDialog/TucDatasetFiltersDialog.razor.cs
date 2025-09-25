namespace WebVella.Tefter.UI.Components;

public partial class TucDatasetFiltersDialog : TfBaseComponent, IDialogContentComponent<TfDataset?>
{
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
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.DataProviderId == Guid.Empty) throw new Exception("DataProviderId is required");
		if (Content.Id == Guid.Empty) _isCreate = true;

		_title = LOC("Manage primary filters");
		_btnText = LOC("Save");
		_iconBtn = TfConstants.GetIcon("Save")!;
		_dataset = Content with { Id = Content.Id };

	}

	private void _onFiltersChanged(List<TfFilterBase> filters)
	{
		_dataset.Filters = filters;
		StateHasChanged();
	}


	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TfDatasetUIService.UpdateDatasetFilters(_dataset.Id, _dataset.Filters);
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
