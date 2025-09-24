using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.Text.RegularExpressions;

namespace WebVella.Tefter.UIServices;

public partial interface ITfSpaceViewUIService
{
	//Events
	event EventHandler<TfSpaceView> SpaceViewCreated;
	event EventHandler<TfSpaceView> SpaceViewUpdated;
	event EventHandler<TfSpaceView> SpaceViewDeleted;

	event EventHandler<List<TfSpaceViewColumn>> SpaceViewColumnsChanged;


	//Space View
	List<TfSpaceView> GetAllSpaceViews(string? search = null);
	List<TfSpaceView> GetSpaceViewsList(Guid spaceId, string? search = null);
	TfSpaceView GetSpaceView(Guid itemId);
	TfSpaceView CreateSpaceView(TfCreateSpaceViewExtended item);
	TfSpaceView UpdateSpaceView(TfCreateSpaceViewExtended item);
	void DeleteSpaceView(Guid itemId);
	void CopySpaceView(Guid itemId);

	//Space View Column
	TfSpaceViewColumnTypeAddonMeta GetSpaceViewColumnTypeById(Guid typeId);
	List<ITfSpaceViewColumnComponentAddon> GetSpaceViewColumnTypeSupportedComponents(Guid typeId);
	ITfSpaceViewColumnComponentAddon GetSpaceViewColumnComponentById(Guid componentId);
	TfSpaceViewColumn GetViewColumn(Guid columnId);
	List<TfSpaceViewColumn> GetViewColumns(Guid viewId);
	void CreateSpaceViewColumn(TfSpaceViewColumn column);
	void UpdateSpaceViewColumn(TfSpaceViewColumn column);
	void RemoveSpaceViewColumn(Guid columnId);
	void MoveSpaceViewColumn(
		Guid viewId,
		Guid columnId,
		bool isUp);


	//Presets
	void UpdateSpaceViewPresets(Guid viewId, List<TfSpaceViewPreset> presets);

	//Export
	ValueTask<byte[]> ExportViewToExcel(TfExportViewData data);
	ValueTask<byte[]> ExportViewToCSV(TfExportViewData data);
}
public partial class TfSpaceViewUIService : ITfSpaceViewUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceViewUIService> LOC;

	public TfSpaceViewUIService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceViewUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfSpaceView> SpaceViewCreated = default!;
	public event EventHandler<TfSpaceView> SpaceViewUpdated = default!;
	public event EventHandler<TfSpaceView> SpaceViewDeleted = default!;

	public event EventHandler<List<TfSpaceViewColumn>> SpaceViewColumnsChanged = default!;
	#endregion

	#region << Space View>>
	public List<TfSpaceView> GetAllSpaceViews(string? search = null)
		=> _tfService.GetAllSpaceViews(search);
	public List<TfSpaceView> GetSpaceViewsList(Guid spaceId, string? search = null)
		=> _tfService.GetSpaceViewsList(spaceId, search);
	public TfSpaceView GetSpaceView(Guid itemId) => _tfService.GetSpaceView(itemId);
	public TfSpaceView CreateSpaceView(TfCreateSpaceViewExtended submit)
	{
		var spaceView = _tfService.CreateSpaceView(submit);
		SpaceViewCreated?.Invoke(this, spaceView);
		return spaceView;
	}
	public TfSpaceView UpdateSpaceView(TfCreateSpaceViewExtended submit)
	{
		if (submit.SpaceDataId is null)
			throw new Exception("SpaceDataId is required");

		var form = new TfSpaceView
		{
			Id = submit.Id,
			Name = submit.Name,
			Type = submit.Type,
			DatasetId = submit.SpaceDataId ?? Guid.Empty,
			SpaceId = submit.SpaceId,
			SettingsJson = JsonSerializer.Serialize(submit.Settings),
			Presets = submit.Presets,
			Position = submit.Position,
		};
		var spaceView = _tfService.UpdateSpaceView(form);
		SpaceViewUpdated?.Invoke(this, spaceView);
		return spaceView;
	}
	public void DeleteSpaceView(Guid itemId)
	{
		var spaceView = _tfService.GetSpaceView(itemId);
		_tfService.DeleteSpaceView(itemId);
		SpaceViewDeleted?.Invoke(this, spaceView);
	}

	public void CopySpaceView(Guid itemId)
	{
		var spaceView = _tfService.CopySpaceView(itemId);
		SpaceViewCreated?.Invoke(this, spaceView);
	}

	#endregion

	#region << Space View Column>>
	public TfSpaceViewColumn GetViewColumn(
		Guid columnId)
		=> _tfService.GetSpaceViewColumn(columnId);

	public List<TfSpaceViewColumn> GetViewColumns(
		Guid viewId)
	=> _tfService.GetSpaceViewColumnsList(viewId) ?? new List<TfSpaceViewColumn>();

	public void CreateSpaceViewColumn(
			TfSpaceViewColumn column)
	{
		_tfService.CreateSpaceViewColumn(column);
		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(column.SpaceViewId));
	}

	public void UpdateSpaceViewColumn(
		TfSpaceViewColumn column)
	{
		_tfService.UpdateSpaceViewColumn(column);
		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(column.SpaceViewId));
	}

	public void RemoveSpaceViewColumn(
		Guid columnId)
	{
		if (columnId == Guid.Empty)
			throw new TfException("Column ID is not specified");

		var column = GetViewColumn(columnId);

		if (column is null)
			throw new TfException("Column is not found");

		_tfService.DeleteSpaceViewColumn(columnId);

		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(column.SpaceViewId));
	}

	public void MoveSpaceViewColumn(
		Guid viewId,
		Guid columnId,
		bool isUp)
	{
		if (columnId == Guid.Empty)
			throw new TfException("Column ID is not specified");
		;
		if (isUp)
			_tfService.MoveSpaceViewColumnUp(columnId);
		else
			_tfService.MoveSpaceViewColumnDown(columnId);

		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(viewId));
	}

	public TfSpaceViewColumnTypeAddonMeta GetSpaceViewColumnTypeById(Guid typeId)
		=> _metaService.GetSpaceViewColumnType(typeId);

	public List<ITfSpaceViewColumnComponentAddon> GetSpaceViewColumnTypeSupportedComponents(Guid typeId)
		=> _metaService.GetSpaceViewColumnTypeSupportedComponents(typeId);

	public ITfSpaceViewColumnComponentAddon GetSpaceViewColumnComponentById(Guid componentId)
		=> _metaService.GetSpaceViewColumnComponent(componentId);
	#endregion

	#region << Presets >>
	public void UpdateSpaceViewPresets(Guid viewId, List<TfSpaceViewPreset> presets)
	{
		_tfService.UpdateSpaceViewPresets(viewId, presets);
		var spaceView = _tfService.GetSpaceView(viewId);
		SpaceViewUpdated?.Invoke(this, spaceView);
	}
	#endregion

	#region << Export >>
	public virtual ValueTask<byte[]> ExportViewToExcel(TfExportViewData data)
	{
		Guid? spaceViewId = null;

		if (data.RouteState.SpaceViewId is not null)
		{
			spaceViewId = data.RouteState.SpaceViewId.Value;
		}
		else if (data.RouteState.SpacePageId is not null)
		{
			var resultNode = _tfService.GetSpacePage(data.RouteState.SpacePageId.Value);
			if (resultNode is null)
				throw new TfException("GetSpaceNode method failed");

			var spacePagesMeta = _metaService.GetSpacePagesComponentsMeta();
			var spacePageMeta = spacePagesMeta.SingleOrDefault(x => x.ComponentId == resultNode.ComponentId);
			if (resultNode.Type == TfSpacePageType.Page && spacePageMeta != null && spacePageMeta.Instance.GetType() == typeof(TucSpaceViewSpacePageAddon))
			{
				try
				{
					var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(resultNode.ComponentOptionsJson);
					spaceViewId = options.SpaceViewId;
				}
				catch (Exception ex)
				{
					throw new Exception($"TfSpaceViewPageComponent options could not deserialize. {ex.Message}");
				}
			}
		}

		if (spaceViewId is null)
			throw new TfException("SpaceViewId not provided");

		var view = _tfService.GetSpaceView(spaceViewId.Value);
		if (view is null)
			throw new TfException("View not found.");


		var viewColumns = _tfService.GetSpaceViewColumnsList(view.Id);
		var spaceData = _tfService.GetDataset(view.DatasetId);
		var allDataProviders = _tfService.GetDataProviders().ToList();
		var allSharedColumns = _tfService.GetSharedColumns();
		var dataProvider = allDataProviders.FirstOrDefault(x => x.Id == spaceData.DataProviderId);
		List<TfFilterBase> filters = data.RouteState.Filters.ConvertQueryFilterToList(viewColumns, allDataProviders, allSharedColumns);
		List<TfSort> sorts = data.RouteState.Sorts.ConvertQuerySortToList(viewColumns); ;

		var viewData = _tfService.QuerySpaceData(
			datasetId: view.DatasetId,
			userFilters: filters,
			userSorts: sorts,
			search: data.RouteState.Search,
			page: null,
			pageSize: null
		);

		using (var workbook = new XLWorkbook())
		{
			var ws = workbook.Worksheets.Add(view.Name);

			var currentExcelRow = 1;
			var currentExcelColumn = 1;
			//Header
			foreach (var column in viewColumns)
			{
				var rangeColumns = 1;
				var endColumn = currentExcelColumn + rangeColumns - 1;
				var cellRange = ws.Range(currentExcelRow, currentExcelColumn, currentExcelRow, endColumn);
				if (rangeColumns > 1) cellRange.Merge();
				cellRange.Value = column.Title;
				currentExcelColumn = currentExcelColumn + rangeColumns;
			}
			currentExcelRow++;

			var typeDict = new Dictionary<string, object>();
			var contextData = new Dictionary<string, object>();
			var compContext = new TfSpaceViewColumnScreenRegionContext(contextData)
			{
				DataTable = viewData,
				Mode = TfComponentPresentationMode.Display, //ignored here
				SpaceViewId = view.Id,
				EditContext = null, //ignored here
				ValidationMessageStore = null, //ignored here
				RowId = Guid.Empty,//set in row loop
				ComponentOptionsJson = null, //set in column loop
				DataMapping = null,//set in column loop
				QueryName = null,//set in column loop
				SpaceViewColumnId = Guid.Empty
			};
			for (int i = 0; i < viewData.Rows.Count; i++)
			{
				var row = viewData.Rows[i];
				var rowId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				currentExcelColumn = 1;
				compContext.RowId = rowId;
				if (data.SelectedRows is not null && data.SelectedRows.Count > 0
					&& !data.SelectedRows.Contains(rowId)) continue;
				foreach (TfSpaceViewColumn column in viewColumns)
				{
					compContext.SpaceViewColumnId = column.Id;
					compContext.ComponentOptionsJson = column.ComponentOptionsJson;
					compContext.DataMapping = column.DataMapping;
					compContext.QueryName = column.QueryName;

					IXLCell excelCell = ws.Cell(currentExcelRow, currentExcelColumn);
					var component = _metaService.GetSpaceViewColumnComponent(column.ComponentId);
					if (component is not null)
					{
						var componentNewInstance = (ITfSpaceViewColumnComponentAddon)Activator.CreateInstance(component.GetType(), compContext);
						componentNewInstance.ProcessExcelCell(_serviceProvider, excelCell);
					}
					currentExcelColumn++;
				}
				currentExcelRow++;
			}

			ws.Columns(1, viewColumns.Count).AdjustToContents();
			MemoryStream ms = new MemoryStream();
			workbook.SaveAs(ms);
			return ValueTask.FromResult(ms.ToArray());
		}
	}
	public virtual ValueTask<byte[]> ExportViewToCSV(TfExportViewData data)
	{
		Guid? spaceViewId = null;
		CultureInfo culture = TfConstants.DefaultCulture;

		if (data.RouteState.SpaceViewId is not null)
		{
			spaceViewId = data.RouteState.SpaceViewId.Value;
		}
		else if (data.RouteState.SpacePageId is not null)
		{
			var resultNode = _tfService.GetSpacePage(data.RouteState.SpacePageId.Value);
			if (resultNode is null)
				throw new TfException("GetSpaceNode method failed");

			var spacePagesMeta = _metaService.GetSpacePagesComponentsMeta();
			var spacePageMeta = spacePagesMeta.SingleOrDefault(x => x.ComponentId == resultNode.ComponentId);
			if (resultNode.Type == TfSpacePageType.Page && spacePageMeta != null && spacePageMeta.Instance.GetType() == typeof(TucSpaceViewSpacePageAddon))
			{
				try
				{
					var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(resultNode.ComponentOptionsJson);
					spaceViewId = options.SpaceViewId;
				}
				catch (Exception ex)
				{
					throw new Exception($"TfSpaceViewPageComponent options could not deserialize. {ex.Message}");
				}
			}
		}

		if (spaceViewId is null)
			throw new TfException("SpaceViewId not provided");

		var view = _tfService.GetSpaceView(spaceViewId.Value);
		if (view is null)
			throw new TfException("View not found.");


		var viewColumns = _tfService.GetSpaceViewColumnsList(view.Id);
		var spaceData = _tfService.GetDataset(view.DatasetId);
		var allDataProviders = _tfService.GetDataProviders().ToList();
		var allSharedColumns = _tfService.GetSharedColumns();
		var dataProvider = allDataProviders.FirstOrDefault(x => x.Id == spaceData.DataProviderId);
		List<TfFilterBase> filters = data.RouteState.Filters.ConvertQueryFilterToList(viewColumns, allDataProviders, allSharedColumns);
		List<TfSort> sorts = data.RouteState.Sorts.ConvertQuerySortToList(viewColumns); ;

		var viewData = _tfService.QuerySpaceData(
			datasetId: view.DatasetId,
			userFilters: filters,
			userSorts: sorts,
			search: data.RouteState.Search,
			page: null,
			pageSize: null
		);

		using var writer = new StringWriter(new StringBuilder());
		var config = new CsvConfiguration(culture)
		{
			HasHeaderRecord = true,
			ShouldQuote = _ => true
		};

		using var csv = new CsvWriter(writer, config);

		foreach (var col in viewColumns)
		{
			csv.WriteField(col.Title);
		}
		csv.NextRecord();

		var typeDict = new Dictionary<string, object>();
		var contextData = new Dictionary<string, object>();
		var compContext = new TfSpaceViewColumnScreenRegionContext(contextData)
		{
			DataTable = viewData,
			Mode = TfComponentPresentationMode.Display, //ignored here
			SpaceViewId = view.Id,
			EditContext = null, //ignored here
			ValidationMessageStore = null, //ignored here
			RowId = Guid.Empty,//set in row loop
			ComponentOptionsJson = null, //set in column loop
			DataMapping = null,//set in column loop
			QueryName = null,//set in column loop
			SpaceViewColumnId = Guid.Empty
		};

		for (int rowIndex = 0; rowIndex < viewData.Rows.Count; rowIndex++)
		{
			var row = viewData.Rows[rowIndex];
			var rowId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
			if (data.SelectedRows is not null && data.SelectedRows.Count > 0
				&& !data.SelectedRows.Contains(rowId)) continue;
			compContext.RowId = rowId;
			foreach (var column in viewColumns)
			{
				compContext.SpaceViewColumnId = column.Id;
				compContext.ComponentOptionsJson = column.ComponentOptionsJson;
				compContext.DataMapping = column.DataMapping;
				compContext.QueryName = column.QueryName;
				var component = _metaService.GetSpaceViewColumnComponent(column.ComponentId);
				string? value = null;
				if (component is not null)
				{
					var componentNewInstance = (ITfSpaceViewColumnComponentAddon)Activator.CreateInstance(component.GetType(), compContext);
					value = componentNewInstance.GetValueAsString(_serviceProvider);
				}
				csv.WriteField(value);
			}
			csv.NextRecord();
		}

		var content = writer.ToString();
		return ValueTask.FromResult(Encoding.UTF8.GetBytes(content));
	}
	#endregion
}
