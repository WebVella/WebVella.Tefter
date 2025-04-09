﻿using ClosedXML.Excel;
using WebVella.Tefter.Web.PageComponents;

namespace WebVella.Tefter.UseCases.Export;

public class ExportUseCase
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfService _tfService;

	public ExportUseCase(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_tfService = serviceProvider.GetService<ITfService>();
	}

	//TODO RUMEN: Move to service method?
	public virtual ValueTask<byte[]> ExportViewToExcel(TucExportViewData data)
	{
		Guid? spaceViewId = null;

		if (data.RouteState.SpaceViewId is not null)
		{
			spaceViewId = data.RouteState.SpaceViewId.Value;
		}
		else if (data.RouteState.SpaceNodeId is not null)
		{
			var resultNode = _tfService.GetSpaceNode(data.RouteState.SpaceNodeId.Value);
			if (resultNode is null)
				throw new TfException("GetSpaceNode method failed");

			if (resultNode.Type == TfSpaceNodeType.Page && resultNode.ComponentType == typeof(TfSpaceViewPageComponent))
			{
				try
				{
					var options = JsonSerializer.Deserialize<TfSpaceViewPageComponentOptions>(resultNode.ComponentOptionsJson);
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

		List<TfFilterBase> filters = null;
		List<TfSort> sorts = null;

		if (data.RouteState.Filters is not null && data.RouteState.Filters.Count > 0)
			filters = data.RouteState.Filters.Select(x => TucFilterBase.ToModel(x)).ToList();

		if (data.RouteState.Sorts is not null && data.RouteState.Sorts.Count > 0)
			sorts = data.RouteState.Sorts.Select(x => x.ToModel()).ToList();

		var viewData = _tfService.QuerySpaceData(
			spaceDataId: view.SpaceDataId,
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
			var compContext = new TucViewColumnComponentContext()
			{
				Hash = Guid.NewGuid(),
				DataTable = viewData,
				Mode = TucComponentMode.Display, //ignored here
				SpaceViewId = view.Id,
				EditContext = null, //ignored here
				ValidationMessageStore = null, //ignored here
				RowIndex = 0,//set in row loop
				CustomOptionsJson = null, //set in column loop
				DataMapping = null,//set in column loop
				QueryName = null,//set in column loop
				SpaceViewColumnId = Guid.Empty
			};
			for (int i = 0; i < viewData.Rows.Count; i++)
			{
				var row = viewData.Rows[i];
				currentExcelColumn = 1;
				compContext.RowIndex = i;
				var rowId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				if (data.SelectedRows is not null && data.SelectedRows.Count > 0
					&& !data.SelectedRows.Contains(rowId)) continue;
				foreach (TfSpaceViewColumn column in viewColumns)
				{
					compContext.SpaceViewColumnId = column.Id;
					compContext.CustomOptionsJson = column.CustomOptionsJson;
					compContext.DataMapping = column.DataMapping;
					compContext.QueryName = column.QueryName;

					IXLCell excelCell = ws.Cell(currentExcelRow, currentExcelColumn);

					if (column.ComponentType.ImplementsInterface(typeof(ITfSpaceViewColumnComponentAddon)))
					{
						var component = (ITfSpaceViewColumnComponentAddon)Activator.CreateInstance(column.ComponentType, compContext);
						component.ProcessExcelCell(_serviceProvider, excelCell);
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

}
