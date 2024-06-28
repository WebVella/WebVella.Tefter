namespace WebVella.Tefter.Web.Components.DataProviderColumnManageDialog;
public partial class TfDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<Tuple<TfDataProviderColumn, TfDataProvider>>
{
	[Inject] private IState<SystemState> SystemState { get; set; }

	[Parameter]
	public Tuple<TfDataProviderColumn, TfDataProvider> Content { get; set; }

	[CascadingParameter]
	public FluentDialog Dialog { get; set; }

	private bool _isCreate = false;
	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private TfDataProviderColumn _form = new();
	private TfDataProvider _provider = new();
	private List<DatabaseColumnTypeInfo> _columnTypes = new();
	private DatabaseColumnTypeInfo _selectedColumnType = null;

	private string value1 = "1";

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Item1 is null) throw new Exception("DataProviderColumn not provided");
		if (Content.Item2 is null) throw new Exception("DataProvider not provided");
		if (Content.Item2.SupportedSourceDataTypes is null
		|| !Content.Item2.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");
		if (Content.Item1.Id == Guid.Empty)
		{
			_title = LOC("Create column");
			_btnText = LOC("Create");
			_iconBtn = new Icons.Regular.Size20.Add();
		}
		else
		{
			_title = LOC("Manage column");
			_btnText = LOC("Save");
			_iconBtn = new Icons.Regular.Size20.Save();
		}
		base.InitForm(_form);
		_provider = Content.Item2;
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
			_columnTypes = SystemState.Value.DataProviderColumnTypes;
			if (!_columnTypes.Any()) throw new Exception("No Data provider column types found in application");
			//Setup form
			if (Content.Item1.Id == Guid.Empty)
			{
				_isCreate = true;
				_form = new TfDataProviderColumn()
				{
					Id = Guid.NewGuid(),
					DataProviderId = Content.Item2.Id,
					SourceType = Content.Item2.SupportedSourceDataTypes.First(),
					CreatedOn = DateTime.Now,
					DbType = _columnTypes.First().Type,
				};
				_selectedColumnType = _columnTypes.First();
			}
			else
			{
				_form = new TfDataProviderColumn()
				{
					Id = Content.Item1.Id,
					DataProviderId = Content.Item1.DataProviderId,
					AutoDefaultValue = Content.Item1.AutoDefaultValue,
					CreatedOn = Content.Item1.CreatedOn,
					DbName = Content.Item1.DbName,
					DbType = Content.Item1.DbType,
					DefaultValue = Content.Item1.DefaultValue,
					IncludeInTableSearch = Content.Item1.IncludeInTableSearch,
					IsNullable = Content.Item1.IsNullable,
					IsSearchable = Content.Item1.IsSearchable,
					IsSortable = Content.Item1.IsSortable,
					IsUnique = Content.Item1.IsUnique,
					PreferredSearchType = Content.Item1.PreferredSearchType,
					SourceName = Content.Item1.SourceName,
					SourceType = Content.Item1.SourceType
				};
				_selectedColumnType = _columnTypes.FirstOrDefault(x=> x.Type == _form.DbType);
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
			if(!_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue)){
				_form.DefaultValue = String.Empty;
			}
			else if(_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue)){
				_form.DefaultValue = null;
			}
			_form.DbType = _selectedColumnType.Type;
			if (_isCreate)
			{
				submitResult = DataProviderManager.CreateDataProviderColumn(_form);
			}
			else
			{
				submitResult = DataProviderManager.UpdateDataProviderColumn(_form);
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

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		//dict["DisplayMode"] = ComponentDisplayMode.Form;
		//dict["Value"] = _form.SettingsJson;
		return dict;
	}

}

