using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Models;

public abstract class TfSpaceViewColumnBaseContext(Dictionary<string, object> viewData) : TfBaseScreenRegionContext
{
	public TfSpaceViewColumn ViewColumn { get; set; } = null!;
	public ITfService TfService { get; init; } = null!;
	public IServiceProvider ServiceProvider { get; init; } = null!;
	
	//When columns need to share data between rows (optimization)
	private Dictionary<string, object> _viewData = viewData;
	public Dictionary<string, object> ViewData { get => _viewData; }	
	
	public TfColor? ForegroundColor { get; set; }	
	public TfColor? BackgroundColor { get; set; }

	public string BodyCellStyles
	{
		get
		{
			var sb = new StringBuilder();
			if(ForegroundColor is not null)
				sb.Append($"--tf-grid-td-color: var(--tf-{ForegroundColor.GetColor().Name}-500);");
			if (BackgroundColor is not null)
				sb.Append($"--tf-grid-td-fill: color-mix(in srgb, var(--neutral-fill-rest), var(--tf-{BackgroundColor.GetColor().Name}-500) 10%);");
			return sb.ToString();
		}		
	}
}

public class TfSpaceViewColumnReadModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext(viewData)
{
	public TfDataTable? DataTable { get; set; } = null;
	public Guid RowId { get; set; } = default;


	public string GetHash()
	{
		var sb = new StringBuilder();
		sb.Append(RowId);
		sb.Append(ViewColumn.Id);
		sb.Append(ViewColumn.TypeOptionsJson);
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

public class TfSpaceViewColumnEditModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext(viewData)
{
	public TfDataTable? DataTable { get; set; } = null;
	public Guid RowId { get; set; } = default;
	public EventCallback<TfSpaceViewColumnDataChange> DataChanged { get; set; }

	public string GetHash()
	{
		var sb = new StringBuilder();
		sb.Append(RowId);
		sb.Append(ViewColumn.Id);
		sb.Append(ViewColumn.TypeOptionsJson);
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

public class TfSpaceViewColumnOptionsModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext(viewData)
{
	public List<ValidationError> ValidationErrors { get; set; } = new();
	public EventCallback<string> SettingsChanged { get; set; }
	public EventCallback<Tuple<string, string?>> DataMappingChanged { get; set; }
}

public class TfSpaceViewColumnExportExcelModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext(viewData)
{
	public TfDataTable? DataTable { get; set; } = null;

	public Guid RowId { get; set; } = default;

	//When columns need to share data between rows (optimization)
	public Dictionary<string, object> ViewData { get; private init; } = viewData;
	public IXLCell ExcelCell { get; set; } = null!;
}

public class TfSpaceViewColumnExportCsvModeContext(Dictionary<string, object> viewData) : TfSpaceViewColumnBaseContext(viewData)
{
	public TfDataTable? DataTable { get; set; } = null;

	public Guid RowId { get; set; } = default;

	//When columns need to share data between rows (optimization)
	public Dictionary<string, object> ViewData { get; private init; } = viewData;
}