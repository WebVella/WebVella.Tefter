using CsvHelper;
using CsvHelper.Configuration;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Task<byte[]> ExportViewToExcel(TfExportViewData data);
	Task<byte[]> ExportViewToCSV(TfExportViewData data);	
}

public partial class TfService : ITfService
{
public virtual Task<byte[]> ExportViewToExcel(TfExportViewData data)
	{
		Guid? spaceViewId = null;

		if (data.RouteState.SpaceViewId is not null)
		{
			spaceViewId = data.RouteState.SpaceViewId.Value;
		}
		else if (data.RouteState.SpacePageId is not null)
		{
			var resultNode = GetSpacePage(data.RouteState.SpacePageId.Value);
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

		var view = GetSpaceView(spaceViewId.Value);
		if (view is null)
			throw new TfException("View not found.");


		var viewColumns = GetSpaceViewColumnsList(view.Id);
		var spaceData = GetDataset(view.DatasetId);
		var allDataProviders = GetDataProviders().ToList();
		var allSharedColumns = GetSharedColumns();
		var dataProvider = allDataProviders.FirstOrDefault(x => x.Id == spaceData.DataProviderId);
		List<TfFilterBase> filters = data.RouteState.Filters.ConvertQueryFilterToList(viewColumns, allDataProviders, allSharedColumns);
		List<TfSort> sorts = data.RouteState.Sorts.ConvertQuerySortToList(viewColumns); ;

		var viewData = QuerySpaceData(
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
			return Task.FromResult(ms.ToArray());
		}
	}
	public virtual Task<byte[]> ExportViewToCSV(TfExportViewData data)
	{
		Guid? spaceViewId = null;
		CultureInfo culture = TfConstants.DefaultCulture;

		if (data.RouteState.SpaceViewId is not null)
		{
			spaceViewId = data.RouteState.SpaceViewId.Value;
		}
		else if (data.RouteState.SpacePageId is not null)
		{
			var resultNode = GetSpacePage(data.RouteState.SpacePageId.Value);
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

		var view = GetSpaceView(spaceViewId.Value);
		if (view is null)
			throw new TfException("View not found.");


		var viewColumns = GetSpaceViewColumnsList(view.Id);
		var spaceData = GetDataset(view.DatasetId);
		var allDataProviders = GetDataProviders().ToList();
		var allSharedColumns = GetSharedColumns();
		var dataProvider = allDataProviders.FirstOrDefault(x => x.Id == spaceData.DataProviderId);
		List<TfFilterBase> filters = data.RouteState.Filters.ConvertQueryFilterToList(viewColumns, allDataProviders, allSharedColumns);
		List<TfSort> sorts = data.RouteState.Sorts.ConvertQuerySortToList(viewColumns); ;

		var viewData = QuerySpaceData(
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
		return Task.FromResult(Encoding.UTF8.GetBytes(content));
	}	
}