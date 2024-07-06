namespace WebVella.Tefter.Web.Components.DataProviderColumnManageDialog;
public partial class TfDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderColumn>
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] private IState<DataProviderAdminState> DataProviderDetailsState { get; set; }
	[Parameter] public TucDataProviderColumn Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isCreate = false;
	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private List<string> _providerTypeOptions = new();
	private List<DatabaseColumnType> _dbTypeOptions = new();


	private Dictionary<int, List<string>> _dbTypeToProviderTypes = new();
	private Dictionary<string, List<TucDatabaseColumnTypeInfo>> _providerTypeToDbTypes = new();
	private Dictionary<int, TucDatabaseColumnTypeInfo> _dbTypeInfoDict = new();

	private string value1 = "1";

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());

		if (Content is null) throw new Exception("Content is null");
		if (DataProviderDetailsState.Value.Provider is null) throw new Exception("DataProvider not provided");
		if (DataProviderDetailsState.Value.Provider.ProviderType.SupportedSourceDataTypes is null
		|| !DataProviderDetailsState.Value.Provider.ProviderType.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");
		
		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? new Icons.Regular.Size20.Add() : new Icons.Regular.Size20.Save();

		foreach (var providerType in DataProviderDetailsState.Value.Provider.ProviderType.SupportedSourceDataTypes)
		{
			if (!_providerTypeToDbTypes.ContainsKey(providerType)) _providerTypeToDbTypes[providerType] = new();

			var dbTypesResult = UC.GetDbTypesForProviderSourceDataTable(DataProviderDetailsState.Value.Provider.ProviderType.Id,providerType);

			foreach (var dbType in dbTypesResult.Value)
			{
				if (!_dbTypeToProviderTypes.ContainsKey(dbType.TypeValue)) _dbTypeToProviderTypes[dbType.TypeValue] = new();

				_dbTypeToProviderTypes[dbType.TypeValue].Add(providerType);
				_providerTypeToDbTypes[providerType].Add(dbType);
			}

		}

		base.InitForm(UC.ColumnForm);

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _loadDataAsync()
	{
		_isBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			//Init type options
			_dbTypeInfoDict = UC.ColumnTypeDict;

			//Setup form
			if (_isCreate)
			{
				//_providerTypeOptions = DataProviderDetailsState.Value.Provider.ProviderType.GetSupportedSourceDataTypes().ToList();
				UC.ColumnForm = new TucDataProviderColumnForm{ 
					Id = Guid.NewGuid(),
					DataProviderId = DataProviderDetailsState.Value.Provider.Id,
					CreatedOn = DateTime.Now,
					SourceType = _providerTypeOptions[0]
				};
				//_dbTypeOptions = _providerTypeToDbTypes[_form.SourceType];

			}
			else
			{
				UC.ColumnForm = new TucDataProviderColumnForm(Content);
				_dbTypeOptions = new();
				//_dbTypeOptions.Add(_form.DbType);
				//_providerTypeOptions = _dbTypeToProviderTypes[_form.DbType];
			}
			base.InitForm(UC.ColumnForm);
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
		}
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);

			MessageStore.Clear();

			////Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			Result<TfDataProvider> submitResult;
			if (!UC.ColumnForm.IsNullable && String.IsNullOrWhiteSpace(UC.ColumnForm.DefaultValue))
			{
				UC.ColumnForm.DefaultValue = String.Empty;
			}
			else if (UC.ColumnForm.IsNullable && String.IsNullOrWhiteSpace(UC.ColumnForm.DefaultValue))
			{
				UC.ColumnForm.DefaultValue = null;
			}

			var submit = UC.ColumnForm.ToModel();

			if (_isCreate)
			{
				submitResult = DataProviderManager.CreateDataProviderColumn(submit);
			}
			else
			{
				submitResult = DataProviderManager.UpdateDataProviderColumn(submit);
			}

			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				await Dialog.CloseAsync(submitResult.Value);
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

	private void _selectionSourceTypeChanged(string option)
	{
		//var dbTypes = _providerTypeToDbTypes[option];
		//if(!dbTypes.Contains(_form.DbType)) 
		//{
		//	_form.DbType = dbTypes[0];
		//}

		//StateHasChanged();
	}

}
