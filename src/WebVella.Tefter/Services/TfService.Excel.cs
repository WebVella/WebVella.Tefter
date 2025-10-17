using CsvHelper;
using CsvHelper.Configuration;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Task<byte[]> ExportViewToExcel(TfExportViewData data);
	Task<byte[]> ExportViewToCSV(TfExportViewData data);
}

public partial class TfService
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
			if (resultNode.Type == TfSpacePageType.Page && spacePageMeta != null &&
			    spacePageMeta.Instance.GetType() == typeof(TucSpaceViewSpacePageAddon))
			{
				try
				{
					var options =
						JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(resultNode.ComponentOptionsJson) ??
						new();
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
		var allDataProviders = GetDataProviders().ToList();
		var allSharedColumns = GetSharedColumns();
		List<TfFilterBase> filters =
			data.RouteState.Filters.ConvertQueryFilterToList(viewColumns, allDataProviders, allSharedColumns);
		List<TfSort> sorts = data.RouteState.Sorts.ConvertQuerySortToList(viewColumns);

		var dataset = QueryDataset(
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
				cellRange.Value = column.Title;
				currentExcelColumn = currentExcelColumn + rangeColumns;
			}

			currentExcelRow++;

			var contextData = new Dictionary<string, object>();
			var compContext = new TfSpaceViewColumnExportExcelModeContext(contextData)
			{
				DataTable = dataset,
				SpaceViewColumn = null!,
				TfService = this,
				RowId = Guid.Empty, //set in row loop
				ExcelCell = null! //set in row loop
			};
			for (int i = 0; i < dataset.Rows.Count; i++)
			{
				var row = dataset.Rows[i];
				var rowId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				currentExcelColumn = 1;
				compContext.RowId = rowId;
				if (data.SelectedRows is not null && data.SelectedRows.Count > 0
				                                  && !data.SelectedRows.Contains(rowId)) continue;
				foreach (TfSpaceViewColumn column in viewColumns)
				{
					compContext.SpaceViewColumn = column;
					compContext.ExcelCell = ws.Cell(currentExcelRow, currentExcelColumn);
					var component = _metaService.GetSpaceViewColumnType(column.TypeId);
					if (component is not null)
					{
						component.ProcessExcelCell(compContext);
					}

					currentExcelColumn++;
				}

				currentExcelRow++;
			}

			ws.Columns(1, viewColumns.Count).AdjustToContents();
			MemoryStream ms = new();
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
			if (resultNode.Type == TfSpacePageType.Page && spacePageMeta != null &&
			    spacePageMeta.Instance.GetType() == typeof(TucSpaceViewSpacePageAddon))
			{
				try
				{
					var options =
						JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(resultNode.ComponentOptionsJson) ??
						new();
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
		var allDataProviders = GetDataProviders().ToList();
		var allSharedColumns = GetSharedColumns();
		List<TfFilterBase> filters =
			data.RouteState.Filters.ConvertQueryFilterToList(viewColumns, allDataProviders, allSharedColumns);
		List<TfSort> sorts = data.RouteState.Sorts.ConvertQuerySortToList(viewColumns);

		var dataset = QueryDataset(
			datasetId: view.DatasetId,
			userFilters: filters,
			userSorts: sorts,
			search: data.RouteState.Search,
			page: null,
			pageSize: null
		);

		using var writer = new StringWriter(new StringBuilder());
		var config = new CsvConfiguration(culture) { HasHeaderRecord = true, ShouldQuote = _ => true };

		using var csv = new CsvWriter(writer, config);

		foreach (var col in viewColumns)
		{
			csv.WriteField(col.Title);
		}

		csv.NextRecord();
		var contextData = new Dictionary<string, object>();
		var compContext = new TfSpaceViewColumnExportCsvModeContext(contextData)
		{
			DataTable = dataset,
			TfService = this,
			SpaceViewColumn = null!, //set in row loop
			RowId = Guid.Empty, //set in row loop
		};

		for (int rowIndex = 0; rowIndex < dataset.Rows.Count; rowIndex++)
		{
			var row = dataset.Rows[rowIndex];
			var rowId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
			if (data.SelectedRows is not null && data.SelectedRows.Count > 0
			                                  && !data.SelectedRows.Contains(rowId)) continue;
			compContext.RowId = rowId;
			foreach (var column in viewColumns)
			{
				compContext.SpaceViewColumn = column;
				var component = _metaService.GetSpaceViewColumnType(column.TypeId);
				string? value = null;
				if (component is not null)
				{
					value = component.GetValueAsString(compContext);
				}

				csv.WriteField(value);
			}

			csv.NextRecord();
		}

		var content = writer.ToString();
		return Task.FromResult(Encoding.UTF8.GetBytes(content));
	}
}