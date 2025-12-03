namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_bookmark")]
public record TfBookmark
{
	[TfDboModelProperty("id")] public Guid Id { get; set; }

	[TfDboModelProperty("name")] public string Name { get; set; }

	[TfDboModelProperty("description")] public string Description { get; set; }

	[TfDboModelProperty("url")] public string Url { get; set; }

	[TfDboModelProperty("created_on")]
	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	public DateTime CreatedOn { get; set; }

	[TfDboModelProperty("user_id")] public Guid UserId { get; set; }

	[TfDboModelProperty("space_page_id")] public Guid SpacePageId { get; set; }

	public TfSpacePage? SpacePage { get; set; } = null;
	public TfSpace? Space { get; set; } = null;

	public string GetUrl()
	{
		if (String.IsNullOrWhiteSpace(Url))
		{
			if(Space is not null && SpacePage is not null)
				return String.Format(TfConstants.SpacePagePageUrl, Space.Id, SpacePage.Id);

			return "#";
		}

		return NavigatorExt.AddQueryValueToUri(Url,TfConstants.ActiveSaveQueryName,Id.ToString());
		
	}

	public List<TfTag> Tags { get; set; } = new();
}

[DboCacheModel]
[TfDboModel("tf_tag")]
public class TfTag
{
	[TfDboModelProperty("id")] public Guid Id { get; set; }

	[TfDboModelProperty("label")] public string Label { get; set; }
}

[DboCacheModel]
[TfDboModel("tf_bookmark_tag")]
internal class TfBookmarkTag
{
	[TfDboModelProperty("bookmark_id")] public Guid BookmarkId { get; set; }

	[TfDboModelProperty("tag_id")] public Guid TagId { get; set; }
}