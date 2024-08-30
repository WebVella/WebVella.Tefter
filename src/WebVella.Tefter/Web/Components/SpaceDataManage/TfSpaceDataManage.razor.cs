using WebVella.Tefter.Web.Components.SpaceDataFilterManageDialog;

namespace WebVella.Tefter.Web.Components.SpaceDataManage;
public partial class TfSpaceDataManage : TfFormBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] private SpaceUseCase UC { get; set; }

	public TucDataProvider SelectedProvider = null;
	public bool _submitting = false;
	public List<string> AllColumnOptions
	{
		get
		{
			if (SelectedProvider is null) return new List<string>();
			return SelectedProvider.ColumnsTotal.Select(x => x.DbName).ToList();
		}
	}
	internal List<string> _columnOptions
	{
		get
		{
			if (UC.SpaceDataManageForm is null || UC.SpaceDataManageForm.Columns is null) return AllColumnOptions;
			return AllColumnOptions.Where(x => !UC.SpaceDataManageForm.Columns.Contains(x)).ToList();
		}
	}

	internal string _selectedColumn = null;
	internal string _selectedFilterColumn = null;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			//Navigator.LocationChanged -= Navigator_LocationChanged;
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		_init();
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
	}

	private void _init()
	{
		UC.SpaceDataManageForm = SpaceState.Value.SpaceData with { Id = SpaceState.Value.SpaceData.Id };
		base.InitForm(UC.SpaceDataManageForm);
		if (UC.SpaceDataManageForm.DataProviderId != Guid.Empty)
		{
			SelectedProvider = UC.AllDataProviders.FirstOrDefault(x => x.Id == UC.SpaceDataManageForm.DataProviderId);
		}
	}

	private void On_StateChanged(SpaceStateChangedAction action)
	{
		_init();
		StateHasChanged();
	}

	private void _dataProviderSelected(TucDataProvider provider)
	{
		if (provider is null) return;
		SelectedProvider = provider;
		UC.SpaceDataManageForm.DataProviderId = SelectedProvider.Id;
	}


	private async Task _addColumn()
	{
		if (_isSubmitting) return;
		try
		{
			if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
			if (UC.SpaceDataManageForm.Columns.Contains(_selectedColumn)) return;



			Result<TucSpaceData> submitResult = UC.AddColumnToSpaceData(SpaceState.Value.SpaceData.Id, _selectedColumn);
			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess("Dataset updated!");
				submitResult.Value.Columns = submitResult.Value.Columns.Order().ToList();
				var spaceDataList = SpaceState.Value.SpaceDataList.ToList();
				var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Value.Id);
				if (itemIndex > -1) spaceDataList[itemIndex] = submitResult.Value;

				Dispatcher.Dispatch(new SetSpaceDataAction(
					spaceData: submitResult.Value,
					spaceDataList: spaceDataList
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			_selectedColumn = null;
			await InvokeAsync(StateHasChanged);
		}

	}


	private async Task _deleteColumn(string column)
	{
		if (_isSubmitting) return;
		try
		{
			Result<TucSpaceData> submitResult = UC.RemoveColumnFromSpaceData(SpaceState.Value.SpaceData.Id, column);
			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess("Dataset updated!");
				var spaceDataList = SpaceState.Value.SpaceDataList.ToList();
				var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Value.Id);
				if (itemIndex > -1) spaceDataList[itemIndex] = submitResult.Value;

				Dispatcher.Dispatch(new SetSpaceDataAction(
					spaceData: submitResult.Value,
					spaceDataList: spaceDataList
				));
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

	public async Task AddFilter(Type type, string dbName, Guid? parentId)
	{
		TucFilterBase filter = null;
		if (type == typeof(TucFilterAnd)) filter = new TucFilterAnd() { ColumnName = dbName };
		else if (type == typeof(TucFilterOr)) filter = new TucFilterOr() { ColumnName = dbName };
		else if (type == typeof(TucFilterBoolean)) filter = new TucFilterBoolean() { ColumnName = dbName };
		else if (type == typeof(TucFilterDateOnly)) filter = new TucFilterDateOnly() { ColumnName = dbName };
		else if (type == typeof(TucFilterDateTime)) filter = new TucFilterDateTime() { ColumnName = dbName };
		else if (type == typeof(TucFilterGuid)) filter = new TucFilterGuid() { ColumnName = dbName };
		else if (type == typeof(TucFilterNumeric)) filter = new TucFilterNumeric() { ColumnName = dbName };
		else if (type == typeof(TucFilterText)) filter = new TucFilterText() { ColumnName = dbName };
		else throw new Exception("Filter type not supported");

		if (parentId is null)
		{
			UC.SpaceDataManageForm.Filters.Add(filter);
		}
		else
		{
			TucFilterBase parentFilter = null;
			foreach (var item in UC.SpaceDataManageForm.Filters)
			{
				var (result, resultParent) = FindFilter(item, parentId.Value, null);
				if (result is not null)
				{
					parentFilter = result;
					break;
				}
			}
			if (parentFilter is not null)
			{
				if (parentFilter is TucFilterAnd) ((TucFilterAnd)parentFilter).Filters.Add(filter);
				if (parentFilter is TucFilterOr) ((TucFilterOr)parentFilter).Filters.Add(filter);
			}
		}

		await _saveFilters();
	}

	public async Task AddColumnFilter(string dbColumn, Guid? parentId)
	{
		if (String.IsNullOrWhiteSpace(dbColumn)) return;
		if (SelectedProvider is null) return;
		var column = SelectedProvider.ColumnsTotal.FirstOrDefault(x => x.DbName == dbColumn);
		if (column is null) return;

		switch (column.DbType.TypeValue)
		{
			case TucDatabaseColumnType.ShortInteger:
			case TucDatabaseColumnType.Integer:
			case TucDatabaseColumnType.LongInteger:
			case TucDatabaseColumnType.Number:
				{
					await AddFilter(typeof(TucFilterNumeric), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Boolean:
				{
					await AddFilter(typeof(TucFilterBoolean), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Date:
				{
					await AddFilter(typeof(TucFilterDateOnly), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.DateTime:
				{
					await AddFilter(typeof(TucFilterDateTime), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.ShortText:
			case TucDatabaseColumnType.Text:
				{
					await AddFilter(typeof(TucFilterText), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Guid:
				{
					await AddFilter(typeof(TucFilterGuid), dbColumn, parentId);
				}
				break;
			default: throw new Exception("Unsupported column data type");

		}
		await InvokeAsync(StateHasChanged);
	}

	public async Task RemoveColumnFilter(Guid filterId)
	{
		TucFilterBase filter = null;
		TucFilterBase parentFilter = null;
		foreach (var item in UC.SpaceDataManageForm.Filters)
		{
			var (result, resultParent) = FindFilter(item, filterId, null);
			if (result is not null)
			{
				filter = result;
				parentFilter = resultParent;
				break;
			}
		}

		if (filter is not null)
		{
			if (parentFilter is null) UC.SpaceDataManageForm.Filters.Remove(filter);
			else if (parentFilter is TucFilterAnd) ((TucFilterAnd)parentFilter).Filters.Remove(filter);
			else if (parentFilter is TucFilterOr) ((TucFilterOr)parentFilter).Filters.Remove(filter);
			await InvokeAsync(StateHasChanged);
			await _saveFilters();
		}
	}

	public async Task UpdateColumnFilter(TucFilterBase filter)
	{
		var filterIndex = UC.SpaceDataManageForm.Filters.FindIndex(x => x.Id == filter.Id);
		if (filterIndex > -1)
		{
			UC.SpaceDataManageForm.Filters[filterIndex] = filter;
			await InvokeAsync(StateHasChanged);
			await _saveFilters();
		}
	}

	public async Task _saveFilters()
	{
		if (_isSubmitting) return;
		try
		{
			Result<TucSpaceData> submitResult = UC.UpdateSpaceDataFilters(SpaceState.Value.SpaceData.Id, UC.SpaceDataManageForm.Filters);
			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess("Dataset updated!");
				var spaceDataList = SpaceState.Value.SpaceDataList.ToList();
				var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Value.Id);
				if (itemIndex > -1) spaceDataList[itemIndex] = submitResult.Value;

				Dispatcher.Dispatch(new SetSpaceDataAction(
					spaceData: submitResult.Value,
					spaceDataList: spaceDataList
				));
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

	public async Task _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedFilterColumn)) return;
		await AddColumnFilter(_selectedFilterColumn, null);
		//_selectedFilterColumn = null; //do not clear for convenience
	}


	private (TucFilterBase, TucFilterBase) FindFilter(TucFilterBase filter, Guid matchId, TucFilterBase parent)
	{
		if (filter.Id == matchId) return (filter, parent);
		List<TucFilterBase> filters = new();
		if (filter is TucFilterAnd) filters = ((TucFilterAnd)filter).Filters;
		if (filter is TucFilterOr) filters = ((TucFilterOr)filter).Filters;
		foreach (var item in filters)
		{
			var (result, resultParent) = FindFilter(item, matchId, filter);
			if (result is not null) return (result, resultParent);
		}
		return (null, null);
	}
}
