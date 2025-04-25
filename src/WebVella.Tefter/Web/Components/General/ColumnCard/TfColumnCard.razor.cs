namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.ColumnCard.TfColumnCard", "WebVella.Tefter")]
public partial class TfColumnCard : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }

	[Parameter]
	public string Title { get; set; } = null;

	[Parameter]
	public TucDataProvider DataProvider { get; set; } = null;

	[Parameter]
	public List<string> Options { get; set; } = null;

	[Parameter]
	public List<string> Items { get; set; } = new();
	[Parameter]
	public EventCallback<List<string>> ItemsChanged { get; set; }

	[Parameter]
	public string NoItemsMessage { get; set; } = "This dataset will return all data provider columns. Select columns for limitation.";

	[Parameter]
	public RenderFragment NoItemsTemplate { get; set; }

	public List<string> AllColumnOptions
	{
		get
		{
			if (DataProvider is not null)
			{
				var result = new List<string>();
				result.AddRange(DataProvider.ColumnsPublic.Select(x => x.DbName).ToList());
				if (_joinedColumns is not null)
					result.AddRange(_joinedColumns.Select(x => x.DbName).Distinct().ToList());

				return result.Order().ToList();
			}
			if (Options is not null) return Options.ToList();
			return new List<string>();
		}
	}
	internal List<string> _columnOptions
	{
		get
		{
			if (Items.Count == 0) return AllColumnOptions;
			return AllColumnOptions.Where(x => !Items.Contains(x)).ToList();
		}
	}

	private string _selectedColumn = null;
	public bool _submitting = false;
	public List<TucColumn> _joinedColumns = new();
	public Guid? _initedProviderId = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _initJoinedProviders();
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		await _initJoinedProviders();
	}

	private async Task _initJoinedProviders()
	{
		if (_initedProviderId == DataProvider?.Id) return;

		_joinedColumns = new();
		if (DataProvider is not null)
		{
			var joinedProviders = (await UC.GetDataProviderJoinedProvidersAsync(DataProvider.Id)) ?? new List<TucDataProvider>();
			var addedHs = new HashSet<string>();
			foreach (var dp in joinedProviders)
			{
				foreach (var col in dp.ColumnsPublic)
				{
					if (addedHs.Contains(col.DbName)) continue;
					addedHs.Add(col.DbName);
					_joinedColumns.Add(col);
				}
			}
		}

		_initedProviderId = DataProvider?.Id;
	}

	private async Task _addColumn()
	{
		if (_submitting) return;

		if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
		if (Items.Contains(_selectedColumn)) return;

		Items.Add(_selectedColumn);

		await ItemsChanged.InvokeAsync(Items);

		_submitting = false;
		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _deleteColumn(string column)
	{
		if (_submitting) return;
		if (!Items.Contains(column)) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
			return;

		Items.Remove(column);
		await ItemsChanged.InvokeAsync(Items);

		_submitting = false;
		await InvokeAsync(StateHasChanged);
	}

	private TucColumn _getProviderColumnByName(string dbName)
	{
		return DataProvider?.ColumnsPublic.FirstOrDefault(x => x.DbName == dbName);
	}

	private TucColumn _getJoinColumnByName(string dbName)
	{
		return _joinedColumns.FirstOrDefault(x => x.DbName == dbName);
	}
}
