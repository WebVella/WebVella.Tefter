namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceView?>
{
	[Parameter] public TfSpaceView? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;

	private TfDataset? _selectedDataset = null;
	private List<string> _generatedColumns = new();
	private int _generatedColumnCountLimit = 20;
	private TfCreateSpaceViewExtended _form = new();
	private ReadOnlyCollection<TfDataProvider> _providers = default!;
	private List<TfDataset> _spaceDataList = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create view") : LOC("Manage view in");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		_spaceDataList = TfUIService.GetDatasets();
		if (_isCreate)
		{
			_form = new TfCreateSpaceViewExtended()
			{
				Id = Guid.NewGuid(),
			};
		}
		else
		{
			_form = new TfCreateSpaceViewExtended()
			{
				Id = Content.Id,
				SpaceDataId = Content.DatasetId,
				Name = Content.Name,
				Presets = Content.Presets,
				Settings = new()
			};
			if (!String.IsNullOrWhiteSpace(Content.SettingsJson) && Content.SettingsJson.StartsWith("{")
			 && Content.SettingsJson.EndsWith("}"))
			{
				_form.Settings = JsonSerializer.Deserialize<TfSpaceViewSettings>(Content.SettingsJson) ?? new();
			}
			_selectedDataset = _spaceDataList.SingleOrDefault(x => x.Id == Content.DatasetId);
		}

		base.InitForm(_form);
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

			TfSpaceView result = default!;
			if (_isCreate)
				result = TfUIService.CreateSpaceView(_form);
			else
				result = TfUIService.UpdateSpaceView(_form);

			await Dialog.CloseAsync(result);
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

	private void _dataSetTypeChangeHandler(TfSpaceViewDatasetType type)
	{
		//_selectedDataProvider = null;
		//_form.DataProviderId = null;
		//_selectedDataset = null;
		//_form.SpaceDataId = null;
		//_form.DatasetType = TfSpaceViewDatasetType.New;

		//if (type == TfSpaceViewDatasetType.New)
		//{
		//	if (_providers.Any())
		//	{
		//		_form.DataProviderId = _providers[0].Id;
		//		_selectedDataProvider = _providers[0];
		//	}
		//	_form.DatasetType = type;
		//}
		//else if (type == TfSpaceViewDatasetType.Existing)
		//{
		//	if (_spaceDataList.Any())
		//	{
		//		_form.SpaceDataId = _spaceDataList[0].Id;
		//		_selectedDataset = _spaceDataList[0];
		//	}
		//	_form.DatasetType = type;
		//}
		//_generatedColumnsListInit();
	}

	private void _dataProviderSelectedHandler(TfDataProvider provider)
	{
		//_selectedDataProvider = null;
		//_form.DataProviderId = null;
		//if (provider is null) return;
		//_selectedDataProvider = provider;
		//_form.DataProviderId = provider.Id;
		//_generatedColumnsListInit();
	}

	private void _datasetSelected(TfDataset dataset)
	{
		_selectedDataset = dataset;
		_form.SpaceDataId = dataset is null ? null : dataset.Id;
		_generatedColumnsListInit();
	}

	private void _columnGeneratorSettingChanged(bool value, string field)
	{

		//if (field == nameof(_form.AddProviderColumns))
		//{
		//	_form.AddProviderColumns = value;
		//}
		//else if (field == nameof(_form.AddSharedColumns))
		//{
		//	_form.AddSharedColumns = value;
		//}
		//else if (field == nameof(_form.AddSystemColumns))
		//{
		//	_form.AddSystemColumns = value;
		//}
		//else if (field == nameof(_form.AddDatasetColumns))
		//{
		//	_form.AddDatasetColumns = value;
		//}
		//_generatedColumnsListInit();
	}

	private void _generatedColumnsListInit()
	{
		//_generatedColumns.Clear();

		//if (_selectedDataProvider is not null)
		//{
		//	if (_form.AddProviderColumns)
		//		_generatedColumns.AddRange(_selectedDataProvider.Columns.Select(x => x.DbName));
		//	if (_form.AddSystemColumns)
		//		_generatedColumns.AddRange(_selectedDataProvider.SystemColumns.Select(x => x.DbName));
		//	if (_form.AddSharedColumns)
		//		_generatedColumns.AddRange(_selectedDataProvider.SharedColumns.Select(x => x.DbName));
		//}
		//else if (_selectedDataset is not null)
		//{
		//	if (_form.AddDatasetColumns)
		//	{
		//		if (_selectedDataset.Columns.Count > 0)
		//		{
		//			_generatedColumns.AddRange(_selectedDataset.Columns.Select(x => x));
		//		}
		//		else if (_providers.Any(x => x.Id == _selectedDataset.DataProviderId))
		//		{
		//			var dataProvider = _providers.Single(x => x.Id == _selectedDataset.DataProviderId);
		//			_generatedColumns.AddRange(dataProvider.Columns.Select(x => x.DbName));
		//		}
		//	}
		//}
	}

}
