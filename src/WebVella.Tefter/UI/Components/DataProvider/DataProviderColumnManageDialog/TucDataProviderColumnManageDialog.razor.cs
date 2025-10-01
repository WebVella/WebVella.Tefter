namespace WebVella.Tefter.UI.Components;

public partial class TucDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProviderColumn?>
{
	[Parameter] public TfDataProviderColumn? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private bool _isCreate = false;
	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private bool _isConnected = false;
	private Icon _iconBtn = null!;
	private TfDataProvider _provider = new();
	private TfUpsertDataProviderColumn _form = new();
	private Dictionary<TfDatabaseColumnType, List<string>> _providerColumnTypeToSourceTypes = new();
	private List<DatabaseColumnTypeInfo> _providerColumnTypeOptions = new();
	private List<TfDataProviderColumnSearchType> _searchTypes = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		//Init Content
		if (Content is null) throw new Exception("Content is null");
		if (Content.DataProviderId == Guid.Empty) throw new Exception("DataProvider not provided");

		//Init provider
		_provider = TfUIService.GetDataProvider(Content.DataProviderId);
		if (_provider is null || _provider.SupportedSourceDataTypes is null
		|| !_provider.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");

		//Init form
		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}

		_providerColumnTypeOptions = TfUIService.GetDatabaseColumnTypeInfosList().ToList();
		_providerColumnTypeToSourceTypes = new();
		foreach (var sourceType in _provider.ProviderType.GetSupportedSourceDataTypes())
		{
			foreach (var providerType in _provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(sourceType))
			{
				if (!_providerColumnTypeToSourceTypes.ContainsKey(providerType))
					_providerColumnTypeToSourceTypes[providerType] = new();

				_providerColumnTypeToSourceTypes[providerType].Add(sourceType);
			}
		}
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		foreach (TfDataProviderColumnSearchType item in Enum.GetValues<TfDataProviderColumnSearchType>())
		{
			_searchTypes.Add(item);
		}
		_isConnected = !String.IsNullOrWhiteSpace(_form.SourceName);
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
			if (_isCreate)
			{
				_form = new TfUpsertDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = _provider.Id,
					CreatedOn = DateTime.Now,
				};
				if (_providerColumnTypeToSourceTypes[_form.DbType].Count > 0)
				{
					_form.SourceType = _providerColumnTypeToSourceTypes[_form.DbType][0];
				}
			}
			else
			{
				_form = Content!.ToUpsert();
				_isConnected = !String.IsNullOrWhiteSpace(_form.SourceName);
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

			var errors = new List<ValidationError>();

			if (_isConnected && String.IsNullOrWhiteSpace(_form.SourceName))
			{
				errors.Add(new ValidationError(nameof(_form.SourceName), LOC("required if attached")));
			}

			foreach (var item in errors)
			{
				MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
			}

			////Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			await Task.Delay(1);

			var submit = new TfUpsertDataProviderColumn
			{
				Id = _form.Id,
				CreatedOn = _form.CreatedOn,
				DataProviderId = _form.DataProviderId,
				DbName = _form.DbName,
				DbType = _form.DbType,
				DefaultValue = _form.DefaultValue,
				IncludeInTableSearch = _form.IncludeInTableSearch,
				SourceName = _form.SourceName,
				SourceType = _form.SourceType,
				RuleSet = _form.RuleSet,
			};

			if (submit.DefaultValue is null
				&& submit.RuleSet == TfDataProviderColumnRuleSet.NullableWithDefault)
				submit.DefaultValue = String.Empty;

			if (!_isConnected)
			{
				submit.SourceName = null;
				submit.SourceType = null;
			}

			submit.FixPrefix(_provider.ColumnPrefix);
			if (_isCreate)
			{
				_provider = TfUIService.CreateDataProviderColumn(submit);
				ToastService.ShowSuccess(LOC("Data provider column was created successfully"));
			}
			else
			{
				_provider = TfUIService.UpdateDataProviderColumn(submit);
				ToastService.ShowSuccess(LOC("Data provider column was updated successfully"));
			}

			await Dialog.CloseAsync(_provider);
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

	private async Task _dbTypeChanged(TfDatabaseColumnType dbType)
	{
		_form.DbType = dbType;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		_form.SourceType = _providerColumnTypeToSourceTypes[_form.DbType][0];
		await InvokeAsync(StateHasChanged);
	}

	private bool _providerTypeSupportsAutogen()
	{
		var dbInfo = _providerColumnTypeOptions.FirstOrDefault(x => x.Type == _form.DbType);
		if (dbInfo is null) return false;
		return dbInfo.SupportAutoDefaultValue;
	}
}
