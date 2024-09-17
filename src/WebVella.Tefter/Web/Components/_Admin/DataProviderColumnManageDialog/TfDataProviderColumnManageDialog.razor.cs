namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.DataProviderColumnManageDialog.TfDataProviderColumnManageDialog","WebVella.Tefter")]
public partial class TfDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderColumn>
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] private IState<TfAppState> TfAppState { get; set; }
	[Parameter] public TucDataProviderColumn Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isCreate = false;
	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private TucDataProviderTypeDataTypeInfo _selectedProviderType = new();
	private List<TucDataProviderTypeDataTypeInfo> _providerTypeOptions = new();

	private TucDatabaseColumnTypeInfo _selectedDbType = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());

		if (Content is null) throw new Exception("Content is null");
		if (TfAppState.Value.AdminManagedDataProvider is null) throw new Exception("DataProvider not provided");
		if (TfAppState.Value.AdminManagedDataProvider.ProviderType.SupportedSourceDataTypes is null
		|| !TfAppState.Value.AdminManagedDataProvider.ProviderType.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");

		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;

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
			//Setup form
			_providerTypeOptions = TfAppState.Value.AdminManagedDataProvider.ProviderType.SupportedSourceDataTypes;
			if (_isCreate)
			{
				_selectedProviderType = null;
				_selectedDbType = null;
				if (_providerTypeOptions.Any())
				{
					_selectedProviderType = _providerTypeOptions[0];
					if (_selectedProviderType.SupportedDatabaseColumnTypes.Any())
						_selectedDbType = _selectedProviderType.SupportedDatabaseColumnTypes[0];
				}

				UC.ColumnForm = new TucDataProviderColumnForm
				{
					Id = Guid.NewGuid(),
					DataProviderId = TfAppState.Value.AdminManagedDataProvider.Id,
					CreatedOn = DateTime.Now,
					PreferredSearchType = UC.SearchTypes.First()
				};
			}
			else
			{
				UC.ColumnForm = new TucDataProviderColumnForm(Content);
				//DbType is readonly so only provider types that support it can be selected
				_selectedDbType = UC.ColumnForm.DbType;
				_selectedProviderType = _providerTypeOptions.FirstOrDefault(x => x.Name == UC.ColumnForm.SourceType);
				_providerTypeOptions = _providerTypeOptions.Where(x => x.SupportedDatabaseColumnTypes.Contains(_selectedDbType)).ToList();
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
			if (!UC.ColumnForm.IsNullable && String.IsNullOrWhiteSpace(UC.ColumnForm.DefaultValue))
			{
				UC.ColumnForm.DefaultValue = String.Empty;
			}
			else if (UC.ColumnForm.IsNullable && String.IsNullOrWhiteSpace(UC.ColumnForm.DefaultValue))
			{
				UC.ColumnForm.DefaultValue = null;
			}

			var submit = UC.ColumnForm with
			{
				SourceType = _selectedProviderType.Name,
				DbType = _selectedDbType
			};
			Result<TucDataProvider> submitResult;
			if (_isCreate)
			{
				submitResult = UC.CreateDataProviderColumn(submit);
			}
			else
			{
				submitResult = UC.UpdateDataProviderColumn(submit);
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

	private void _selectionSourceTypeChanged(TucDataProviderTypeDataTypeInfo option)
	{
		_selectedProviderType = option;
		_selectedDbType = null;
		if (_selectedProviderType.SupportedDatabaseColumnTypes.Any())
			_selectedDbType = _selectedProviderType.SupportedDatabaseColumnTypes[0];
		StateHasChanged();
	}

}
