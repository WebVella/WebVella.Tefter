namespace WebVella.Tefter.Models;

public class TfComponentContext
{
	public Guid SpaceViewId { get; set; }
	public Guid? SelectedAddonId { get; set; } = null;
	public string QueryName { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string CustomOptionsJson { get; set; } = "{}";
	public TfComponentMode Mode { get; set; } = TfComponentMode.Display;
	public TfDataTable DataTable { get; set; } = null;
	public int RowIndex { get; set; } = -1;
}
