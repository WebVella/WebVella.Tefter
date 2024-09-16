namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewManageDialog.TfSpaceViewManageDialog", "WebVella.Tefter")]
public partial class TfSpaceViewManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceView>
{
	[Inject] private SpaceUseCase UC { get; set; }
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

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.SpaceId == Guid.Empty) throw new Exception("SpaceId is required");
		await UC.Init(this.GetType(), Content.SpaceId);
		base.InitForm(UC.SpaceViewManageForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create view in {0}", UC.SpaceName) : LOC("Manage view in {0}", UC.SpaceName);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;

		if (_isCreate)
		{
			UC.SpaceViewManageForm = new TucSpaceView()
			{
				Id = Guid.NewGuid(),
				SpaceId = Content.SpaceId,
			};
			if (UC.AllDataProviders.Any())
			{
				UC.SpaceViewManageForm.DataProviderId = UC.AllDataProviders[0].Id;
				_selectedDataProvider = UC.AllDataProviders[0];
			}
			_generatedColumnsListInit();
		}
		else
		{
			UC.SpaceViewManageForm = Content with { Id = Content.Id, DataSetType = TucSpaceViewDataSetType.Existing };
			_selectedDataset = UC.SpaceDataList.Single(x => x.Id == Content.SpaceDataId);
		}

		base.InitForm(UC.SpaceViewManageForm);
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

			Result<TucSpaceView> result = null;
			if (_isCreate)
				result = UC.CreateSpaceViewWithForm(UC.SpaceViewManageForm);
			else
				result = UC.UpdateSpaceViewWithForm(UC.SpaceViewManageForm);

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
			{
				await Dialog.CloseAsync(result.Value);
			}
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

	private void _dataSetTypeChangeHandler(TucSpaceViewDataSetType type)
	{
		_selectedDataProvider = null;
		UC.SpaceViewManageForm.DataProviderId = null;
		_selectedDataset = null;
		UC.SpaceViewManageForm.SpaceDataId = null;
		UC.SpaceViewManageForm.DataSetType = TucSpaceViewDataSetType.New;

		if (type == TucSpaceViewDataSetType.New)
		{
			if (UC.AllDataProviders.Any())
			{
				UC.SpaceViewManageForm.DataProviderId = UC.AllDataProviders[0].Id;
				_selectedDataProvider = UC.AllDataProviders[0];
			}
			UC.SpaceViewManageForm.DataSetType = type;
		}
		else if (type == TucSpaceViewDataSetType.Existing)
		{
			if (UC.SpaceDataList.Any()){ 
				UC.SpaceViewManageForm.SpaceDataId = UC.SpaceDataList[0].Id;
				_selectedDataset = UC.SpaceDataList[0];			
			}
			UC.SpaceViewManageForm.DataSetType = type;
		}
		_generatedColumnsListInit();
	}

	private void _dataProviderSelectedHandler(string providerIdString)
	{
		_selectedDataProvider = null;
		UC.SpaceViewManageForm.DataProviderId = null;
		Guid providerId = Guid.Empty;
		if (!String.IsNullOrWhiteSpace(providerIdString) && Guid.TryParse(providerIdString, out providerId)) ;
		if (providerId == Guid.Empty) return;

		var provider = UC.AllDataProviders.FirstOrDefault(x => x.Id == providerId);
		if (provider is null) return;
		_selectedDataProvider = provider;
		UC.SpaceViewManageForm.DataProviderId = provider.Id;
		_generatedColumnsListInit();
	}

	private void _datasetSelected(TucSpaceData dataset)
	{
		_selectedDataset = dataset;
		UC.SpaceViewManageForm.SpaceDataId = dataset is null ? null : dataset.Id;
		_generatedColumnsListInit();
	}

	private void _columnGeneratorSettingChanged(bool value, string field)
	{

		if (field == nameof(UC.SpaceViewManageForm.AddProviderColumns))
		{
			UC.SpaceViewManageForm.AddProviderColumns = value;
		}
		else if (field == nameof(UC.SpaceViewManageForm.AddSharedColumns))
		{
			UC.SpaceViewManageForm.AddSharedColumns = value;
		}
		else if (field == nameof(UC.SpaceViewManageForm.AddSystemColumns))
		{
			UC.SpaceViewManageForm.AddSystemColumns = value;
		}
		else if (field == nameof(UC.SpaceViewManageForm.AddDatasetColumns))
		{
			UC.SpaceViewManageForm.AddDatasetColumns = value;
		}
		_generatedColumnsListInit();
	}

	private void _generatedColumnsListInit()
	{
		_generatedColumns.Clear();

		if (_selectedDataProvider is not null)
		{
			if (UC.SpaceViewManageForm.AddProviderColumns)
				_generatedColumns.AddRange(_selectedDataProvider.Columns.Select(x => x.DbName));
			if (UC.SpaceViewManageForm.AddSystemColumns)
				_generatedColumns.AddRange(_selectedDataProvider.SystemColumns.Select(x => x.DbName));
			if (UC.SpaceViewManageForm.AddSharedColumns)
				_generatedColumns.AddRange(_selectedDataProvider.SharedColumns.Select(x => x.DbName));
		}
		else if (_selectedDataset is not null)
		{
			if (UC.SpaceViewManageForm.AddDatasetColumns)
				_generatedColumns.AddRange(_selectedDataset.Columns.Select(x => x));
		}
	}
}
