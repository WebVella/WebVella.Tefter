namespace WebVella.Tefter.Models;
public class TfSpaceViewSettings
{
	public int? FreezeStartingNColumns { get; set; } = 0;
	public int? FreezeFinalNColumns { get; set; } = 0;
	public bool CanCreateRows { get; set; } = false;
	public bool CanUpdateRows { get; set; } = false;
	public bool CanDeleteRows { get; set; } = false;
}
