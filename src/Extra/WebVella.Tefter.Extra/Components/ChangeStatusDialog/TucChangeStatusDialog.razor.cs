using System.Text.Json;
using WebVella.Tefter.UI.EventsBus;

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
    private TfSelectOption? _initialStep = null;
    private TfSelectOption? _initalStatus = null;
    private TfSelectOption? _selectedStep = null;
    private TfSelectOption? _selectedStatus = null;
    private readonly Guid _stepDataProviderId = new Guid(TfExtraConstants.STEPS_DATA_PROVIDER_ID);
    private readonly Guid _statusDataProviderId = new Guid(TfExtraConstants.STATUS_DATA_PROVIDER_ID);
    private readonly string _stepColumnName = $"{TfExtraConstants.IH001_ID_DATA_IDENTITY}.{TfExtraConstants.STEP_SHARED_COLUMN_NAME}";
    private readonly string _statusColumnName = $"{TfExtraConstants.IH001_ID_DATA_IDENTITY}.{TfExtraConstants.STATUS_SHARED_COLUMN_NAME}";
    private TfDataProvider _stepProvider = null!;
    private TfDataProvider _statusProvider = null!;
    
    
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
        _stepProvider = TfService.GetDataProvider(_stepDataProviderId)!;
        _statusProvider = TfService.GetDataProvider(_statusDataProviderId)!;        
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
                    Label = (string)row[$"{_statusProvider.ColumnPrefix}name"]!, Value = (string)row[$"{_statusProvider.ColumnPrefix}code"]!,
                });
            }

            allStatuses = allStatuses.OrderBy(x => x.Label).ToList();
            foreach (TfDataRow row in stepDt.Rows)
            {
                var stepCode = (string)row[$"{_stepProvider.ColumnPrefix}code"]!;
                _stepOptions.Add(new TfSelectOption()
                {
                    Label = (string)row[$"{_stepProvider.ColumnPrefix}name"]!, 
                    Value = stepCode,
                    IconName = (string?)row[$"{_stepProvider.ColumnPrefix}icon"]!,
                });
                _stepStatusOptionsDict[stepCode] = allStatuses.Where(x => ((string)x.Value!).StartsWith(stepCode)).ToList();
            }

            _stepOptions = _stepOptions.OrderBy(x => x.Label).ToList();

            //If all rows are with the same data for step and status preselect it
            List<string> stepData = new();
            List<string> statusData = new();
            foreach (TfDataRow row in Content!.Data.Rows)
            {
                var step = (string?)row[_stepColumnName];
                var status = (string?)row[_statusColumnName];
                if (!String.IsNullOrWhiteSpace(step) && !stepData.Contains(step)) stepData.Add(step);
                if (!String.IsNullOrWhiteSpace(status) && !statusData.Contains(status)) statusData.Add(status);
            }

            if (stepData.Count >= 1)
            {
                _selectedStep = _stepOptions.FirstOrDefault(x => (string)x.Value! == stepData[0]);
                if (_selectedStep is not null)
                {
                    _initialStep = _selectedStep with { Value = _selectedStep.Value };

                    _statusOptions = _stepStatusOptionsDict[(string)_selectedStep.Value!];
                    if (statusData.Count == 1)
                    {
                        _selectedStatus = _statusOptions.FirstOrDefault(x => (string)x.Value! == statusData[0]);

                        if (_selectedStatus is not null)
                            _initalStatus = _selectedStatus with { Value = _selectedStep.Value };
                    }
                }
            }

            if (_initialStep is null)
            {
                _initialStep = _stepOptions.FirstOrDefault(x => (string)x.Value! == "IH001-STP1");
                _initalStatus = _statusOptions.FirstOrDefault(x => (string)x.Value! == "IH001-STP1-STS1");
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

            //Publish Talk event to fill in
            var eventObj = new
            {
                ChannelId = new Guid("27a7703a-8fe8-4363-aee1-64a219d7520e"),
                Type = 0,
                Content =
                    $"Status changed <dl>" +
                    $"<dt>from</dt><dd style=\"color:var(--accent-foreground-rest)\">{_initialStep?.Label ?? "N/A"} :: {_initalStatus?.Label ?? "N/A"}</dd>" +
                    $"<dt>to</dt><dd style=\"color:var(--accent-foreground-rest)\">{_selectedStep?.Label ?? "N/A"} :: {_selectedStatus?.Label ?? "N/A"}</dd>" +
                    $"</dl>",
                UserId = TfAuthLayout.GetUserId(),
                RowIds = submitDict.Keys.ToList(),
                DataProviderId = Content.Dataset.DataProviderId
            };
            await TfEventBus.PublishAsync(
                key: TfAuthLayout.GetSessionId(),
                payload: new TfAddonEventPayload(new Guid("27a7703a-8fe8-4363-aee1-64a219d7520e"),
                    "ITalkService-CreateThread-WithRowIdModel", JsonSerializer.Serialize(eventObj))
            );


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