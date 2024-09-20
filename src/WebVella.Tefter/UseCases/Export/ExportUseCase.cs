using ClosedXML.Excel;

namespace WebVella.Tefter.UseCases.Login;

public class ExportUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly IDataManager _dataManager;
	private readonly ITfSpaceManager _spaceManager;
	public ExportUseCase(
		IIdentityManager identityManager,
		IDataManager dataManager,
		ITfSpaceManager spaceManager
		)
	{
		_identityManager = identityManager;
		_dataManager = dataManager;
		_spaceManager = spaceManager;

	}

	//TODO RUMEN: Move to service method?
	public async ValueTask<byte[]> ExportViewToExcel(TucExportViewData data)
	{
		if (data.RouteState.SpaceViewId is null) throw new Exception("SpaceViewId not provided");

		var viewSrvResult = _spaceManager.GetSpaceView(data.RouteState.SpaceViewId.Value);
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
			additionalFilters: filters,
			sortOverrides: sorts,
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
			var compContext = new TfComponentContext()
			{
				DataTable = viewData,
				Mode = TfComponentMode.Display, //ignored here
				SpaceViewId = view.Id,
				EditContext = null, //ignored here
				ValidationMessageStore = null, //ignored here
				RowIndex = 0,//set in row loop
				CustomOptionsJson = null, //set in column loop
				DataMapping = null,//set in column loop
				SelectedAddonId = null,//set in column loop
				QueryName = null,//set in column loop
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
					compContext.CustomOptionsJson = column.CustomOptionsJson;
					compContext.DataMapping = column.DataMapping;
					compContext.SelectedAddonId = column.SelectedAddonId;
					compContext.QueryName = column.QueryName;

					var rangeColumns = 1;
					var endColumn = currentExcelColumn + rangeColumns - 1;
					var cellRange = ws.Range(currentExcelRow, currentExcelColumn, currentExcelRow, endColumn);
					if (rangeColumns > 1) cellRange.Merge();
					cellRange.Style.NumberFormat.Format = null;
					cellRange.Value = "";
					currentExcelColumn = currentExcelColumn + rangeColumns;

					if (column.ComponentType is not null
						&& column.ComponentType.GetInterface(nameof(ITfExportableViewColumn)) != null)
					{
						var component = (ITfExportableViewColumn)Activator.CreateInstance(column.ComponentType, compContext);
						var exportData = component.GetExportData();
						cellRange.Style.NumberFormat.Format = exportData.Format;
						cellRange.Value = exportData.Value;
					}



				}
				currentExcelRow++;
			}

			ws.Columns(1, viewColumns.Count).AdjustToContents();
			MemoryStream ms = new MemoryStream();
			workbook.SaveAs(ms);
			return ms.ToArray();
		}
	}


}
