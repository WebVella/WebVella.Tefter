using ClosedXML.Excel;
using WebVella.Tefter.Web.PageComponents;

namespace WebVella.Tefter.UseCases.Export;

public class ExportUseCase
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IIdentityManager _identityManager;
	private readonly ITfDataManager _dataManager;
	private readonly ITfSpaceManager _spaceManager;
	public ExportUseCase(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_identityManager = serviceProvider.GetService<IIdentityManager>();
		_dataManager = serviceProvider.GetService<ITfDataManager>();
		_spaceManager = serviceProvider.GetService<ITfSpaceManager>();

	}

	//TODO RUMEN: Move to service method?
	public virtual ValueTask<byte[]> ExportViewToExcel(TucExportViewData data)
	{
		Guid? spaceViewId = null;
		if (data.RouteState.SpaceViewId is not null) spaceViewId = data.RouteState.SpaceViewId.Value;
		else if (data.RouteState.SpaceNodeId is not null)
		{
			var resultNode = _spaceManager.GetSpaceNode(data.RouteState.SpaceNodeId.Value);
			if (resultNode.IsFailed) throw new Exception("GetSpaceNode method failed");
			if (resultNode.Value.Type == TfSpaceNodeType.Page && resultNode.Value.ComponentType == typeof(TfSpaceViewPageComponent))
			{
				try
				{
					var options = JsonSerializer.Deserialize<TfSpaceViewPageComponentOptions>(resultNode.Value.ComponentOptionsJson);
					spaceViewId = options.SpaceViewId;

				}
				catch (Exception ex)
				{
					throw new Exception($"TfSpaceViewPageComponent options could not be deserialized. {ex.Message}");
				}
			}
		}

		if (spaceViewId is null) throw new Exception("SpaceViewId not provided");

		var viewSrvResult = _spaceManager.GetSpaceView(spaceViewId.Value);
		if (viewSrvResult.IsFailed) throw new Exception("GetSpaceView method failed");

		var view = viewSrvResult.Value;
		if (view is null) throw new Exception("View not found");

		var viewColumnsSrvResult = _spaceManager.GetSpaceViewColumnsList(view.Id);
		if (viewColumnsSrvResult.IsFailed) throw new Exception("GetSpaceViewColumnsList method failed");
		var viewColumns = viewColumnsSrvResult.Value;

		List<TfFilterBase> filters = null;
		List<TfSort> sorts = null;

		if (data.RouteState.Filters is not null
			&& data.RouteState.Filters.Count > 0) filters = data.RouteState.Filters.Select(x => TucFilterBase.ToModel(x)).ToList();

		if (data.RouteState.Sorts is not null
			&& data.RouteState.Sorts.Count > 0)
			sorts = data.RouteState.Sorts.Select(x => x.ToModel()).ToList();

		var dataServiceResult = _dataManager.QuerySpaceData(
			spaceDataId: view.SpaceDataId,
			userFilters: filters,
			userSorts: sorts,
			search: data.RouteState.Search,
			page: null,
			pageSize: null
		);
		if (dataServiceResult.IsFailed)
			throw new Exception("QuerySpaceData method failed");
		var viewData = dataServiceResult.Value;

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

					if (column.ComponentType is not null
						&& column.ComponentType.GetInterface(nameof(ITucExcelExportableViewColumn)) != null)
					{
						var component = (ITucExcelExportableViewColumn)Activator.CreateInstance(column.ComponentType, compContext);
						component.ProcessExcelCell(_serviceProvider,excelCell);
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
