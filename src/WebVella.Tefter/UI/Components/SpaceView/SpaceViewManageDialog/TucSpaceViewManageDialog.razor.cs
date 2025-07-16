namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewManageDialog.TfSpaceViewManageDialog", "WebVella.Tefter")]
public partial class TucSpaceViewManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceView?>
{
	[Inject] protected ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] protected ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] protected ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] protected ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Parameter] public TfSpaceView? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;

	private TfDataProvider? _selectedDataProvider = null;
	private TfSpaceData? _selectedDataset = null;
	private TfSpace _space = default!;
	private List<string> _generatedColumns = new();
	private int _generatedColumnCountLimit = 20;
	private TfCreateSpaceViewExtended _form = new();
	private ReadOnlyCollection<TfDataProvider> _providers = default!;
	private List<TfSpaceData> _spaceDataList = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.SpaceId == Guid.Empty) throw new Exception("SpaceId is required");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_space = TfSpaceUIService.GetSpace(Content.SpaceId);
		if (_space is null) throw new Exception("Space is null");
		_title = _isCreate ? LOC("Create view in {0}", _space.Name) : LOC("Manage view in {0}", _space.Name);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		_providers = TfDataProviderUIService.GetDataProviders();
		_spaceDataList = TfSpaceDataUIService.GetSpaceDataList(_space.Id);
		if (_isCreate)
		{
			_form = new TfCreateSpaceViewExtended()
			{
				Id = Guid.NewGuid(),
				SpaceId = Content.SpaceId,
			};
			if (_providers.Any())
			{
				_form.DataProviderId = _providers[0].Id;
				_selectedDataProvider = _providers[0];
			}
			_generatedColumnsListInit();
		}
		else
		{
			_form = new TfCreateSpaceViewExtended() { Id = Content.Id, DataSetType = TfSpaceViewDataSetType.Existing };
			if (!String.IsNullOrWhiteSpace(Content.SettingsJson) && Content.SettingsJson.StartsWith("{")
			 && Content.SettingsJson.EndsWith("}"))
			{
				_form.Settings = JsonSerializer.Deserialize<TfSpaceViewSettings>(Content.SettingsJson) ?? new();
			}
			_selectedDataset = _spaceDataList.SingleOrDefault(x => x.Id == Content.SpaceDataId);
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
				result = TfSpaceViewUIService.CreateSpaceView(_form);
			else
				result = TfSpaceViewUIService.UpdateSpaceView(_form);

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

	private void _dataSetTypeChangeHandler(TfSpaceViewDataSetType type)
	{
		_selectedDataProvider = null;
		_form.DataProviderId = null;
		_selectedDataset = null;
		_form.SpaceDataId = null;
		_form.DataSetType = TfSpaceViewDataSetType.New;

		if (type == TfSpaceViewDataSetType.New)
		{
			if (_providers.Any())
			{
				_form.DataProviderId = _providers[0].Id;
				_selectedDataProvider = _providers[0];
			}
			_form.DataSetType = type;
		}
		else if (type == TfSpaceViewDataSetType.Existing)
		{
			if (_spaceDataList.Any())
			{
				_form.SpaceDataId = _spaceDataList[0].Id;
				_selectedDataset = _spaceDataList[0];
			}
			_form.DataSetType = type;
		}
		_generatedColumnsListInit();
	}

	private void _dataProviderSelectedHandler(TfDataProvider provider)
	{
		_selectedDataProvider = null;
		_form.DataProviderId = null;
		if (provider is null) return;
		_selectedDataProvider = provider;
		_form.DataProviderId = provider.Id;
		_generatedColumnsListInit();
	}

	private void _datasetSelected(TfSpaceData dataset)
	{
		_selectedDataset = dataset;
		_form.SpaceDataId = dataset is null ? null : dataset.Id;
		_generatedColumnsListInit();
	}

	private void _columnGeneratorSettingChanged(bool value, string field)
	{

		if (field == nameof(_form.AddProviderColumns))
		{
			_form.AddProviderColumns = value;
		}
		else if (field == nameof(_form.AddSharedColumns))
		{
			_form.AddSharedColumns = value;
		}
		else if (field == nameof(_form.AddSystemColumns))
		{
			_form.AddSystemColumns = value;
		}
		else if (field == nameof(_form.AddDataSetColumns))
		{
			_form.AddDataSetColumns = value;
		}
		_generatedColumnsListInit();
	}

	private void _generatedColumnsListInit()
	{
		_generatedColumns.Clear();

		if (_selectedDataProvider is not null)
		{
			if (_form.AddProviderColumns)
				_generatedColumns.AddRange(_selectedDataProvider.Columns.Select(x => x.DbName));
			if (_form.AddSystemColumns)
				_generatedColumns.AddRange(_selectedDataProvider.SystemColumns.Select(x => x.DbName));
			if (_form.AddSharedColumns)
				_generatedColumns.AddRange(_selectedDataProvider.SharedColumns.Select(x => x.DbName));
		}
		else if (_selectedDataset is not null)
		{
			if (_form.AddDataSetColumns)
			{
				if (_selectedDataset.Columns.Count > 0)
				{
					_generatedColumns.AddRange(_selectedDataset.Columns.Select(x => x));
				}
				else if (_providers.Any(x => x.Id == _selectedDataset.DataProviderId))
				{
					var dataProvider = _providers.Single(x => x.Id == _selectedDataset.DataProviderId);
					_generatedColumns.AddRange(dataProvider.Columns.Select(x => x.DbName));
				}
			}
		}
	}

}
