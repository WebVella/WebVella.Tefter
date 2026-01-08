namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewChangeDatasetDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceView?>
{
	[Parameter] public TfSpaceView? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private TfSpaceView _form = new();
	private List<TfDataset> _allDatasets = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) throw new Exception("Content Id is required");
		if (Content.DatasetId == Guid.Empty) throw new Exception("Content DatasetId is required");
		_form = new TfSpaceView { Id = Content.Id, DatasetId = Content.DatasetId, };
		_allDatasets = TfService.GetDatasets();
		base.InitForm(_form);
	}

	private void _datasetChanged(TfDataset? dataset)
	{
		if(dataset is null) return;
		_form.DatasetId = dataset.Id;
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);
			//Columns should not be generated on edit
			MessageStore.Clear();


			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			var spaceView = TfService.GetSpaceView(Content!.Id);
			spaceView!.DatasetId = _form.DatasetId;
			TfService.UpdateSpaceView(spaceView);
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpaceViewUpdatedEventPayload(Content!));		
			await Dialog.CloseAsync();
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
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