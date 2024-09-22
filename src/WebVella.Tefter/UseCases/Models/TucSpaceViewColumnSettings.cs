namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceViewColumnSettings
{
	public short? Width { get; set; }
	public OfficeColor Color { get; set; } = OfficeColor.Default;
	public OfficeColor BackgroundColor { get; set; } = OfficeColor.Default;


}
