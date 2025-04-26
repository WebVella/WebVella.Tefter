using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Models;

public class TfSpaceViewColumnScreenRegionContext : TfBaseScreenRegionContext
{
	public Guid SpaceViewId { get; set; }
	public Guid SpaceViewColumnId { get; set; }
	public string QueryName { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string CustomOptionsJson { get; set; } = "{}";
	public TfComponentPresentationMode Mode { get; set; } = TfComponentPresentationMode.Display;
	public TfDataTable DataTable { get; set; } = null;
	public int RowIndex { get; set; } = -1;
	public EditContext EditContext { get; set; } = null;
	public ValidationMessageStore ValidationMessageStore { get; set; } = null;

	public string GetHash(){ 
		var sb = new StringBuilder();
		sb.Append(RowIndex);
		sb.Append(SpaceViewId);
		sb.Append(SpaceViewColumnId);
		sb.Append(DataTable.QueryInfo);
		sb.Append(DataTable.QueryInfo.DataProviderId);
		sb.Append(DataTable.QueryInfo.SpaceDataId);
		return sb.ToString();
	}
}
