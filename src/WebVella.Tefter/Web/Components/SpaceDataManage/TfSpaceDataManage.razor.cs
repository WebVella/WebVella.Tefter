using WebVella.Tefter.Web.Components.SpaceDataFilterManageDialog;

namespace WebVella.Tefter.Web.Components.SpaceDataManage;
public partial class TfSpaceDataManage : TfFormBaseComponent
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public TucSpaceData Form { get; set; }

	public TucDataProvider SelectedProvider = null;
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
			if (Form is null || Form.Columns is null) return AllColumnOptions;
			return AllColumnOptions.Where(x => !Form.Columns.Contains(x)).ToList();
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

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		base.InitForm(Form);
		if (Form is null) throw new Exception("Form is null");
		if(Form.DataProviderId != Guid.Empty){ 
			SelectedProvider = UC.AllDataProviders.FirstOrDefault(x=> x.Id == Form.DataProviderId);
		}
	}

	private async Task _dataProviderSelected(TucDataProvider provider)
	{
		if (provider is null) return;
		SelectedProvider = provider;
		Form.DataProviderId = SelectedProvider.Id;
	}


	private void _addColumn()
	{
		try
		{
			if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
			if (Form.Columns.Contains(_selectedColumn)) return;
			Form.Columns.Add(_selectedColumn);
			Form.Columns = Form.Columns.Order().ToList();
		}
		finally
		{
			_selectedColumn = null;
		}
	}

	public void AddFilter(Type type, string dbName, Guid? parentId)
	{
		TucFilterBase filter = null;
		if (type == typeof(TucFilterAnd)) filter = new TucFilterAnd(){ColumnName = dbName};
		else if (type == typeof(TucFilterOr)) filter = new TucFilterOr(){ColumnName = dbName};
		else if (type == typeof(TucFilterBoolean)) filter = new TucFilterBoolean(){ColumnName = dbName};
		else if (type == typeof(TucFilterDateOnly)) filter = new TucFilterDateOnly(){ColumnName = dbName};
		else if (type == typeof(TucFilterDateTime)) filter = new TucFilterDateTime(){ColumnName = dbName};
		else if (type == typeof(TucFilterGuid)) filter = new TucFilterGuid(){ColumnName = dbName};
		else if (type == typeof(TucFilterNumeric)) filter = new TucFilterNumeric(){ColumnName = dbName};
		else if (type == typeof(TucFilterText)) filter = new TucFilterText(){ColumnName = dbName};
		else throw new Exception("Filter type not supported");

		if (parentId is null)
		{
			Form.Filters.Add(filter);
		}
		else
		{
			TucFilterBase parentFilter = null;
			foreach (var item in Form.Filters)
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
		StateHasChanged();
	}

	public void AddColumnFilter(string dbColumn, Guid? parentId)
	{
		if (String.IsNullOrWhiteSpace(dbColumn)) return;
		if(SelectedProvider is null) return;
		var column = SelectedProvider.ColumnsTotal.FirstOrDefault(x => x.DbName == dbColumn);
		if (column is null) return;

		switch (column.DbType.TypeValue)
		{
			case TucDatabaseColumnType.ShortInteger:
			case TucDatabaseColumnType.Integer:
			case TucDatabaseColumnType.LongInteger:
			case TucDatabaseColumnType.Number:
				{
					AddFilter(typeof(TucFilterNumeric), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Boolean:
				{
					AddFilter(typeof(TucFilterBoolean), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Date:
				{
					AddFilter(typeof(TucFilterDateOnly), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.DateTime:
				{
					AddFilter(typeof(TucFilterDateTime), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.ShortText:
			case TucDatabaseColumnType.Text:
				{
					AddFilter(typeof(TucFilterText), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Guid:
				{
					AddFilter(typeof(TucFilterGuid), dbColumn, parentId);
				}
				break;
			default: throw new Exception("Unsupported column data type");

		}

		StateHasChanged();
	}

	public void RemoveColumnFilter(Guid filterId)
	{
		TucFilterBase filter = null;
		TucFilterBase parentFilter = null;
		foreach (var item in Form.Filters)
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
			if (parentFilter is null) Form.Filters.Remove(filter);
			else if (parentFilter is TucFilterAnd) ((TucFilterAnd)parentFilter).Filters.Remove(filter);
			else if (parentFilter is TucFilterOr) ((TucFilterOr)parentFilter).Filters.Remove(filter);
			StateHasChanged();
		}
	}

	public void _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedFilterColumn)) return;
		AddColumnFilter(_selectedFilterColumn, null);
		//_selectedFilterColumn = null; //do not clear for convenience
	}

	private void _deleteColumn(string column)
	{
		if (String.IsNullOrWhiteSpace(column)) return;
		if (!Form.Columns.Contains(column)) return;
		Form.Columns.Remove(column);
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
