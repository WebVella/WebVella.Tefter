namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewManageDialog.TfSpaceViewManageDialog", "WebVella.Tefter")]
public partial class TfSpaceViewManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceView>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucSpaceView Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;

	private TucDataProvider _selectedDataProvider = null;
	private TucSpaceData _selectedDataset = null;
	private List<string> _generatedColumns = new();
	private int _generatedColumnCountLimit = 20;
	private TucSpaceView _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.SpaceId == Guid.Empty) throw new Exception("SpaceId is required");
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create view in {0}", TfAppState.Value.Space.Name) : LOC("Manage view in {0}", TfAppState.Value.Space.Name);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);

		if (_isCreate)
		{
			_form = new TucSpaceView()
			{
				Id = Guid.NewGuid(),
				SpaceId = Content.SpaceId,
			};
			if (TfAppState.Value.AllDataProviders.Any())
			{
				_form.DataProviderId = TfAppState.Value.AllDataProviders[0].Id;
				_selectedDataProvider = TfAppState.Value.AllDataProviders[0];
			}
			_generatedColumnsListInit();
		}
		else
		{
			_form = Content with { Id = Content.Id, DataSetType = TucSpaceViewDataSetType.Existing };
			_selectedDataset = TfAppState.Value.SpaceDataList.SingleOrDefault(x => x.Id == Content.SpaceDataId);
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

			Tuple<TucSpaceView, TucSpaceData> result = null;
			if (_isCreate)
				result = UC.CreateSpaceViewWithForm(_form);
			else
				result = UC.UpdateSpaceViewWithForm(_form);

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

	private void _dataSetTypeChangeHandler(TucSpaceViewDataSetType type)
	{
		_selectedDataProvider = null;
		_form.DataProviderId = null;
		_selectedDataset = null;
		_form.SpaceDataId = null;
		_form.DataSetType = TucSpaceViewDataSetType.New;

		if (type == TucSpaceViewDataSetType.New)
		{
			if (TfAppState.Value.AllDataProviders.Any())
			{
				_form.DataProviderId = TfAppState.Value.AllDataProviders[0].Id;
				_selectedDataProvider = TfAppState.Value.AllDataProviders[0];
			}
			_form.DataSetType = type;
		}
		else if (type == TucSpaceViewDataSetType.Existing)
		{
			if (TfAppState.Value.SpaceDataList.Any())
			{
				_form.SpaceDataId = TfAppState.Value.SpaceDataList[0].Id;
				_selectedDataset = TfAppState.Value.SpaceDataList[0];
			}
			_form.DataSetType = type;
		}
		_generatedColumnsListInit();
	}

	private void _dataProviderSelectedHandler(TucDataProvider provider)
	{
		_selectedDataProvider = null;
		_form.DataProviderId = null;
		if (provider is null) return;
		_selectedDataProvider = provider;
		_form.DataProviderId = provider.Id;
		_generatedColumnsListInit();
	}

	private void _datasetSelected(TucSpaceData dataset)
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
		else if (field == nameof(_form.AddDatasetColumns))
		{
			_form.AddDatasetColumns = value;
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
			if (_form.AddDatasetColumns)
			{
				if (_selectedDataset.Columns.Count > 0)
				{
					_generatedColumns.AddRange(_selectedDataset.Columns.Select(x => x));
				}
				else if (TfAppState.Value.AllDataProviders.Any(x => x.Id == _selectedDataset.DataProviderId))
				{
					var dataProvider = TfAppState.Value.AllDataProviders.Single(x => x.Id == _selectedDataset.DataProviderId);
					_generatedColumns.AddRange(dataProvider.Columns.Select(x => x.DbName));
				}
			}
		}
	}

}
