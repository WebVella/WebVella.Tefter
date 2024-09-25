namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceViewSettings
{
	public int FreezeStartingNColumns { get; set; } = 0;
	public int FreezeFinalNColumns { get; set; } = 0;
}
