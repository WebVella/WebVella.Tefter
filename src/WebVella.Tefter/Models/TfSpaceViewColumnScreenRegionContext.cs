using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Models;

public abstract class TfSpaceViewColumnBaseContext : TfBaseScreenRegionContext
{
	public TfSpaceViewColumn SpaceViewColumn { get; set; } = null!;
	public ITfService TfService { get; init; } = null!;
}

public class TfSpaceViewColumnReadModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext
{
	//When columns need to share data between rows (optimization)
	private Dictionary<string, object> _viewData = viewData;
	public Dictionary<string, object> ViewData { get => _viewData; }
	public TfDataTable? DataTable { get; set; } = null;
	public Guid RowId { get; set; } = default;
	
	
	public string GetHash()
	{
		var sb = new StringBuilder();
		sb.Append(RowId);
		sb.Append(SpaceViewColumn.Id);
		sb.Append(SpaceViewColumn.TypeOptionsJson);
		if (DataTable is not null)
		{
			sb.Append(DataTable.GetHashCode());
			sb.Append(DataTable.QueryInfo);
			sb.Append(DataTable.QueryInfo.DataProviderId);
			sb.Append(DataTable.QueryInfo.SpaceDataId);
		}

		return sb.ToString();
	}
}

public class TfSpaceViewColumnEditModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext
{
	//When columns need to share data between rows (optimization)
	private Dictionary<string, object> _viewData = viewData;
	public Dictionary<string, object> ViewData { get => _viewData; }
	public TfDataTable? DataTable { get; set; } = null;
	public Guid RowId { get; set; } = default;
	public EventCallback<TfSpaceViewColumnDataChange> DataChanged { get; set; }
	public string GetHash()
	{
		var sb = new StringBuilder();
		sb.Append(RowId);
		sb.Append(SpaceViewColumn.Id);
		sb.Append(SpaceViewColumn.TypeOptionsJson);
		if (DataTable is not null)
		{
			sb.Append(DataTable.GetHashCode());
			sb.Append(DataTable.QueryInfo);
			sb.Append(DataTable.QueryInfo.DataProviderId);
			sb.Append(DataTable.QueryInfo.SpaceDataId);
		}

		return sb.ToString();
	}
}

public class TfSpaceViewColumnOptionsModeContext : TfSpaceViewColumnBaseContext
{
	public EditContext? EditContext { get; set; } = null;
	public ValidationMessageStore? ValidationMessageStore { get; set; } = null;

	// public EventCallback<Tuple<string,string>> DataMappingChanged { get; set; }
	// public EventCallback<string> OptionsChanged { get; set; }	
}

public class TfSpaceViewColumnExportExcelModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext
{
	public TfDataTable? DataTable { get; set; } = null;

	public Guid RowId { get; set; } = default;

	//When columns need to share data between rows (optimization)
	public Dictionary<string, object> ViewData { get; private init; } = viewData;
	public IXLCell ExcelCell { get; set; } = null!;
}

public class TfSpaceViewColumnExportCsvModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext
{
	public TfDataTable? DataTable { get; set; } = null;

	public Guid RowId { get; set; } = default;

	//When columns need to share data between rows (optimization)
	public Dictionary<string, object> ViewData { get; private init; } = viewData;
}