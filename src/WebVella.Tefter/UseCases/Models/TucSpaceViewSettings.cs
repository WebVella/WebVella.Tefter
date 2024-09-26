namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceViewSettings
{
	public int? FreezeStartingNColumns { get; set; } = 0;
	public int? FreezeFinalNColumns { get; set; } = 0;
	public bool CanCreateRows { get; set; } = false;
	public bool CanDeleteRows { get; set; } = false;
}
