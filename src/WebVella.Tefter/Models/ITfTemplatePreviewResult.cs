namespace WebVella.Tefter.Models;
public interface ITfTemplatePreviewResult
{
	public List<ValidationError> Errors { get; set; }
}
