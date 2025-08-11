namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_bookmark")]
public record TfBookmark
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("description")]
	public string Description { get; set; }

	[TfDboModelProperty("url")]
	public string Url { get; set; }

	[TfDboModelProperty("created_on")]
	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	public DateTime CreatedOn { get; set; }

	[TfDboModelProperty("user_id")]
	public Guid UserId { get; set; }

	[TfDboModelProperty("space_view_id")]
	public Guid SpaceViewId { get; set; }

	public List<TfTag> Tags { get; set; } = new();
}

[DboCacheModel]
[TfDboModel("tf_tag")]
public class TfTag
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("label")]
	public string Label { get; set; }

}

[DboCacheModel]
[TfDboModel("tf_bookmark_tag")]
internal class TfBookmarkTag
{
	[TfDboModelProperty("bookmark_id")]
	public Guid BookmarkId { get; set; }

	[TfDboModelProperty("tag_id")]
	public Guid TagId { get; set; }

}