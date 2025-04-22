namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumnSettings
{
	public short? Width { get; set; }
	public TfColor Color { get; set; } = TfColor.Black;
	public TfColor BackgroundColor { get; set; } = TfColor.Black;


}
