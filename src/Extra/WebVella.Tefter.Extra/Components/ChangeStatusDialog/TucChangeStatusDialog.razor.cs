namespace WebVella.Tefter.Extra.Components;

public partial class TucChangeStatusDialog : TfBaseComponent, IDialogContentComponent<TucChangeStatusDialogModel?>
{
	[Parameter] public TucChangeStatusDialogModel? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;

	private bool _initialized = false;
	private List<TfSelectOption> _stepOptions = new();
	private readonly Dictionary<string, List<TfSelectOption>> _stepStatusOptionsDict = new();
	private List<TfSelectOption> _statusOptions = new();
	private TfSelectOption? _selectedStep = null;
	private TfSelectOption? _selectedStatus = null;
	private readonly Guid _stepDataProviderId = new Guid("cba9f436-1b82-42fb-8e3e-6213d667fb73");
	private readonly Guid _statusDataProviderId = new Guid("437c9cb1-7f76-4a05-a9b3-c90a714d9768");
	private readonly string _stepColumnName = "ih001_id.sc_ih001_step";
	private readonly string _statusColumnName = "ih001_id.sc_ih001_status";
	
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.SelectedRowIds is null) throw new Exception("SelectedRowIds is required");
		if (Content.Dataset is null) throw new Exception("Dataset is required");
		if (Content.SpaceView is null) throw new Exception("SpaceView is required");
		if (Content.User is null) throw new Exception("User is required");
		if (Content.SpacePage is null) throw new Exception("SpacePage is required");
		if (Content.Data is null) throw new Exception("Data is required");
		_initOptions();
	}

	private void _initOptions()
	{
		_statusOptions.Clear();
		if (!_initialized)
		{
			_stepOptions.Clear();
			_stepStatusOptionsDict.Clear();
			var stepDt = TfService.QueryDataProvider(providerId: _stepDataProviderId);
			var statusDt = TfService.QueryDataProvider(providerId: _statusDataProviderId);
			var allStatuses = new List<TfSelectOption>();
			foreach (TfDataRow row in statusDt.Rows)
			{
				allStatuses.Add(new TfSelectOption()
				{
					Label = (string)row["dp3_name"]!, Value = (string)row["dp3_id"]!,
				});
			}
			allStatuses = allStatuses.OrderBy(x => x.Label).ToList();
			foreach (TfDataRow row in stepDt.Rows)
			{
				var stepId = (string)row["dp2_id"]!;
				_stepOptions.Add(new TfSelectOption() { Label = (string)row["dp2_name"]!, Value = stepId, });
				_stepStatusOptionsDict[stepId] = allStatuses.Where(x => ((string)x.Value!).StartsWith(stepId)).ToList();
			}
			_stepOptions = _stepOptions.OrderBy(x => x.Label).ToList();
			
			//If all rows are with the same data for step and status preselect it
			List<string> stepData = new();
			List<string> statusData = new();
			foreach (TfDataRow row in Content!.Data.Rows)
			{
				var step = (string?)row[_stepColumnName];
				var status = (string?)row[_statusColumnName];
				if(!String.IsNullOrWhiteSpace(step) && !stepData.Contains(step)) stepData.Add(step);
				if(!String.IsNullOrWhiteSpace(status) && !statusData.Contains(status)) statusData.Add(status);
			}
			if (stepData.Count == 1)
			{
				_selectedStep = _stepOptions.FirstOrDefault(x => (string)x.Value! == stepData[0]);
				if (_selectedStep is not null)
				{
					_statusOptions = _stepStatusOptionsDict[(string)_selectedStep.Value!];
					if(statusData.Count == 1)
						_selectedStatus = _statusOptions.FirstOrDefault(x => (string)x.Value! == statusData[0]);					
				}


			}

			_initialized = true;
		}
	}

	private async Task _stepChanged(TfSelectOption? step)
	{
		if (step is not null && (string?)step.Value == (string?)_selectedStep?.Value) return;
		Console.WriteLine($"Status changed: {step.Value}");
		_selectedStep = step;
		_statusOptions = _stepStatusOptionsDict[(string)step!.Value!];
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		_selectedStatus = _statusOptions[0];
		await InvokeAsync(StateHasChanged);
	}

	private void _statusChanged(TfSelectOption? status)
	{
		if (status is not null && (string?)status.Value == (string?)_selectedStatus?.Value) return;
		Console.WriteLine($"Status changed: {status.Value}");
		_selectedStatus = status;
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			if (_selectedStep is null)
				_error = LOC("Please select a Process step");
			if (_selectedStatus is null)
				_error = LOC("Please select a Step status");

			if (!String.IsNullOrWhiteSpace(_error))
			{
				return;
			}

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var submitDict = new Dictionary<Guid, Dictionary<string, object?>>();
			foreach (var rowId in Content!.SelectedRowIds)
			{
				submitDict[rowId] = new()
				{
					[_stepColumnName] = (string)_selectedStep!.Value!, 
					[_statusColumnName] = (string)_selectedStatus!.Value!
				};
			}
			
			_ = TfService.UpdateDatasetRows(
				datasetId: Content.Dataset.Id,
				rowsDict: submitDict
			);
			ToastService.ShowSuccess(LOC($"Process status changed"));
			await TfEventBus.PublishAsync(
				key: TfAuthLayout.GetSessionId(),
				payload: new TfSpaceViewDataReloadEventPayload(Content.SpaceView.Id));
			await Dialog.CloseAsync();
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

public record TucChangeStatusDialogModel
{
	public List<Guid> SelectedRowIds { get; init; } = new();
	public TfDataTable Data { get; init; } = null!;
	public TfDataset Dataset { get; init; } = null!;
	public TfSpaceView SpaceView { get; init; } = null!;
	public TfSpacePage SpacePage { get; init; } = null!;
	public TfUser User { get; init; } = null!;
}