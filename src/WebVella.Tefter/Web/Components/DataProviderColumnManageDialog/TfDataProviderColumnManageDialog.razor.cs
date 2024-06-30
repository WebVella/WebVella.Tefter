namespace WebVella.Tefter.Web.Components.DataProviderColumnManageDialog;
public partial class TfDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProviderColumn>
{
	[Inject] private IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

	[Parameter]
	public TfDataProviderColumn Content { get; set; }

	[CascadingParameter]
	public FluentDialog Dialog { get; set; }

	private bool _isCreate = false;
	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private TfDataProviderColumnManageDialogForm _form = new();

	private List<string> _providerTypeOptions = new();
	private List<DatabaseColumnType> _dbTypeOptions = new();


	private Dictionary<DatabaseColumnType, List<string>> _dbTypeToProviderTypes = new();
	private Dictionary<string, List<DatabaseColumnType>> _providerTypeToDbTypes = new();
	private Dictionary<DatabaseColumnType, DatabaseColumnTypeInfo> _dbTypeInfoDict = new();

	private string value1 = "1";

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Content is null) throw new Exception("Content is null");
		if (DataProviderDetailsState.Value.Provider is null) throw new Exception("DataProvider not provided");
		if (DataProviderDetailsState.Value.Provider.SupportedSourceDataTypes is null
		|| !DataProviderDetailsState.Value.Provider.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");
		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? new Icons.Regular.Size20.Add() : new Icons.Regular.Size20.Save();

		foreach (var providerType in DataProviderDetailsState.Value.Provider.ProviderType.GetSupportedSourceDataTypes())
		{
			if (!_providerTypeToDbTypes.ContainsKey(providerType)) _providerTypeToDbTypes[providerType] = new();

			var dbTypes = DataProviderDetailsState.Value.Provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(providerType);

			foreach (var dbType in dbTypes)
			{
				if (!_dbTypeToProviderTypes.ContainsKey(dbType)) _dbTypeToProviderTypes[dbType] = new();

				_dbTypeToProviderTypes[dbType].Add(providerType);
				_providerTypeToDbTypes[providerType].Add(dbType);
			}

		}

		base.InitForm(_form);

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
			throw new NotImplementedException();
			//_dbTypeInfoDict = SystemState.Value.DataProviderColumnTypes.ToDictionary(x => x.Type);

			//Setup form
			if (_isCreate)
			{
				_providerTypeOptions = DataProviderDetailsState.Value.Provider.ProviderType.GetSupportedSourceDataTypes().ToList();
				_form = new TfDataProviderColumnManageDialogForm()
				{
					Id = Guid.NewGuid(),
					DataProviderId = DataProviderDetailsState.Value.Provider.Id,
					CreatedOn = DateTime.Now,
					SourceType = _providerTypeOptions[0]
				};
				_dbTypeOptions = _providerTypeToDbTypes[_form.SourceType];

			}
			else
			{
				_form = TfDataProviderColumnManageDialogForm.FromModel(Content);
				_dbTypeOptions = new();
				_dbTypeOptions.Add(_form.DbType);
				_providerTypeOptions = _dbTypeToProviderTypes[_form.DbType];
			}
			base.InitForm(_form);
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
			if (!_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue))
			{
				_form.DefaultValue = String.Empty;
			}
			else if (_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue))
			{
				_form.DefaultValue = null;
			}

			var submit = TfDataProviderColumnManageDialogForm.ToModel(_form);

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
		var dbTypes = _providerTypeToDbTypes[option];
		if(!dbTypes.Contains(_form.DbType)) 
		{
			_form.DbType = dbTypes[0];
		}

		StateHasChanged();
	}

}

public class TfDataProviderColumnManageDialogForm
{
	public Guid Id { get; set; }

	public Guid DataProviderId { get; set; }

	public string SourceName { get; set; }

	public string SourceType { get; set; }

	public DateTime CreatedOn { get; set; }

	public string DbName { get; set; }

	public DatabaseColumnType DbType { get; set; } = DatabaseColumnType.Text;

	public string DefaultValue { get; set; }

	public bool AutoDefaultValue { get; set; }

	public bool IsNullable { get; set; }

	public bool IsUnique { get; set; }

	public bool IsSortable { get; set; }

	public bool IsSearchable { get; set; }

	public TfDataProviderColumnSearchType PreferredSearchType { get; set; }

	public bool IncludeInTableSearch { get; set; }

	public static TfDataProviderColumnManageDialogForm FromModel(TfDataProviderColumn model)
	{
		var form = new TfDataProviderColumnManageDialogForm
		{
			AutoDefaultValue = model.AutoDefaultValue,
			IsNullable = model.IsNullable,
			DbName = model.DbName,
			SourceName = model.SourceName,
			SourceType = model.SourceType,
			DefaultValue = model.DefaultValue,
			CreatedOn = model.CreatedOn,
			DataProviderId = model.DataProviderId,
			DbType = model.DbType,
			Id = model.Id,
			IncludeInTableSearch = model.IncludeInTableSearch,
			IsSearchable = model.IsSearchable,
			IsSortable = model.IsSortable,
			IsUnique = model.IsUnique,
			PreferredSearchType = model.PreferredSearchType,
		};

		return form;
	}
	public static TfDataProviderColumn ToModel(TfDataProviderColumnManageDialogForm form)
	{
		var model = new TfDataProviderColumn()
		{
			Id = form.Id,
			AutoDefaultValue = form.AutoDefaultValue,
			IsNullable = form.IsNullable,
			PreferredSearchType = form.PreferredSearchType,
			IsUnique = form.IsUnique,
			IsSortable = form.IsSortable,
			IsSearchable = form.IsSearchable,
			IncludeInTableSearch = form.IncludeInTableSearch,
			CreatedOn = form.CreatedOn,
			DataProviderId = form.DataProviderId,
			DbName = form.DbName,
			DbType = form.DbType,
			DefaultValue = form.DefaultValue,
			SourceName = form.SourceName,
			SourceType = form.SourceType,
		};

		return model;
	}
}

