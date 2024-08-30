using System.Text;

namespace WebVella.Tefter.Web.Models;

public record RouteData
{
	public Dictionary<int,string> SegmentsByIndexDict { get; set; }
	public Guid? SpaceId { get; set; }
	public Guid? SpaceDataId { get; set; }
	public Guid? SpaceViewId { get; set; }
	public Guid? UserId { get; set; }
	public Guid? DataProviderId { get; set; }
	public string SpaceSection { get; set; }
}
