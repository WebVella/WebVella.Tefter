namespace WebVella.Tefter.Models;

public record TfSpaceViewColumnSettings
{
	public short? Width { get; set; }
	public TfColor Color { get; set; } = TfColor.Black;
	public TfColor BackgroundColor { get; set; } = TfColor.Black;


}
