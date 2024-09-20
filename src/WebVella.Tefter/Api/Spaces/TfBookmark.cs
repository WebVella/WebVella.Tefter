namespace WebVella.Tefter;

[DboCacheModel]
[DboModel("bookmark")]
public class TfBookmark
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboModelProperty("description")]
	public string Description { get; set; }

	[DboModelProperty("url")]
	public string Url { get; set; }

	[DboModelProperty("created_on")]
	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	public DateTime CreatedOn { get; set; }

	[DboModelProperty("user_id")]
	public Guid UserId { get; set; }

	[DboModelProperty("space_view_id")]
	public Guid SpaceViewId { get; set; }

	public List<TfTag> Tags { get; set; } = new();
}

[DboCacheModel]
[DboModel("tag")]
public class TfTag
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("label")]
	public string Label { get; set; }

}

[DboCacheModel]
[DboModel("bookmark_tags")]
internal class TfBookmarkTag
{
	[DboModelProperty("bookmark_id")]
	public Guid BookmarkId { get; set; }

	[DboModelProperty("tag_id")]
	public Guid TagId { get; set; }

}