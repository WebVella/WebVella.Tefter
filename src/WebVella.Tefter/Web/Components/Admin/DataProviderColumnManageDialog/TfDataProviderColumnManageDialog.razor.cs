namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderColumnManageDialog.TfDataProviderColumnManageDialog", "WebVella.Tefter")]
public partial class TfDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderColumn>
{
	[Inject] private AppStateUseCase UC { get; set; }
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
	private TucDataProviderColumnForm _form = new();
	private TucDataProviderTypeDataTypeInfo _selectedProviderType = new();
	private List<TucDataProviderTypeDataTypeInfo> _providerTypeOptions = new();
	private List<TucDataProviderColumnSearchTypeInfo> _searchTypes = new();

	private TucDatabaseColumnTypeInfo _selectedDbType = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (TfAppState.Value.AdminDataProvider is null) throw new Exception("DataProvider not provided");
		if (TfAppState.Value.AdminDataProvider.ProviderType.SupportedSourceDataTypes is null
		|| !TfAppState.Value.AdminDataProvider.ProviderType.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");

		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}
		_title = _isCreate ? LOC("Create column 1") : LOC("Manage column 1");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		foreach (TfDataProviderColumnSearchType item in Enum.GetValues<TfDataProviderColumnSearchType>())
		{
			_searchTypes.Add(new TucDataProviderColumnSearchTypeInfo(item));
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
			//Setup form
			_providerTypeOptions = TfAppState.Value.AdminDataProvider.ProviderType.SupportedSourceDataTypes;
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

				_form = new TucDataProviderColumnForm
				{
					Id = Guid.NewGuid(),
					DataProviderId = TfAppState.Value.AdminDataProvider.Id,
					CreatedOn = DateTime.Now,
					PreferredSearchType = _searchTypes.First()
				};
			}
			else
			{
				_form = new TucDataProviderColumnForm(Content);
				//DbType is readonly so only provider types that support it can be selected
				_selectedDbType = _form.DbType;
				_selectedProviderType = _providerTypeOptions.FirstOrDefault(x => x.Name == _form.SourceType);
				_providerTypeOptions = _providerTypeOptions.Where(x => x.SupportedDatabaseColumnTypes.Contains(_selectedDbType)).ToList();
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
			if (!_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue))
			{
				_form.DefaultValue = String.Empty;
			}
			else if (_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue))
			{
				_form.DefaultValue = null;
			}

			var submit = _form with
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
