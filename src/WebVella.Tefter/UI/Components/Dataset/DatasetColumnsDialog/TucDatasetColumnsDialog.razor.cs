namespace WebVella.Tefter.UI.Components;

public partial class TucDatasetColumnsDialog : TfBaseComponent, IDialogContentComponent<TfDataset?>
{
	[Parameter] public TfDataset? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;

	private TfDataset _dataset = new();
	private TfDataProvider _provider = null!;
	private List<TfDatasetColumn> _items = new();
	List<TfDatasetColumn> _allOptions = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.DataProviderId == Guid.Empty) throw new Exception("DataProviderId is required");

		_provider = TfService.GetDataProvider(Content.DataProviderId);
		if (_provider is null) throw new Exception("DataProviderId not found");

		_title = LOC("Manage columns");
		_btnText = LOC("Save");
		_iconBtn = TfConstants.GetIcon("Save")!;
		_dataset = Content with { Id = Content.Id };

		_items = new List<TfDatasetColumn>();
		_allOptions = TfService.GetDatasetColumnOptions(_dataset.Id);
		foreach (var column in _dataset.Columns)
		{
			var option = _allOptions.FirstOrDefault(x => x.ColumnName == column);
			if (option != null)
				_items.Add(option);
		}

		foreach (var identity in _dataset.Identities)
		{
			foreach (var column in identity.Columns)
			{
				var option = _allOptions.FirstOrDefault(x => x.DataIdentity == identity.DataIdentity && x.SourceColumnName == column);
				if (option != null)
					_items.Add(option);
			}
		}

	}

	private async Task _onItemsChanged(List<TfDatasetColumn> columns)
	{
		_items = columns.ToList();
	}


	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TfService.UpdataDatasetColumns(_dataset.Id, _items);
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfDatasetUpdatedEventPayload(_dataset));	
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
