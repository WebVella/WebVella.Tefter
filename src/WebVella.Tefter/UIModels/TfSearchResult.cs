using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public class TfSearchResult
{
	public Guid Id { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public TfSearchResultType Type { get; set; } = TfSearchResultType.SpaceView;
	public DateTime? CreatedOn { get; set; }
	public Guid? SpaceViewId { get; set; }
	public Guid? SpaceId { get; set; }
	public string? SpaceViewName { get; set; }
	public string? SpaceName { get; set; }
	public TfColor SpaceColor { get; set; } = TfConstants.DefaultThemeColor;
	public string? SpaceIcon { get; set; } = null;
	public string? Url { get; set; }
	public List<TfTag> Tags { get; set; } = new();
}
public enum TfSearchResultType
{
	[Description("space view")]
	SpaceView = 0,
	[Description("bookmark")]
	Bookmark = 1,
	[Description("url save")]
	UrlSave = 2
}
