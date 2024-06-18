namespace WebVella.Tefter.Web.Models;

public record UrlData
{
	public Guid? SpaceId { get; set; }
	public Guid? SpaceDataId { get; set; }
	public Guid? SpaceViewId { get; set; }
	public Guid? UserId { get; set; }
}
