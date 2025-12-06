using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_bookmark")]
public record TfBookmark
{
	[TfDboModelProperty("id")] public Guid Id { get; set; }
	//identity_row_id - is autocreated
	[TfDboModelProperty("name")] public string Name { get; set; } = null!;
	[TfDboModelProperty("description")] public string? Description { get; set; } = null;

	[TfDboModelProperty("created_on")]
	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	public DateTime CreatedOn { get; set; }

	[TfDboModelProperty("user_id")] public Guid UserId { get; set; }

	[TfDboModelProperty("space_page_id")] public Guid SpacePageId { get; set; }

	[TfDboModelProperty("type")]
	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfBookmarkType>))]
	public TfBookmarkType Type { get; set; } = TfBookmarkType.URL;

	//URL type
	[TfDboModelProperty("url")] public string? Url { get; set; } = null;

	//DataProviderRows
	[TfDboModelProperty("data_identity")] public string? DataIdentity { get; set; } = null;


	//Filled in on demand
	public TfSpacePage? SpacePage { get; set; } = null;
	public TfSpace? Space { get; set; } = null;

	public string GetUrl()
	{
		if (String.IsNullOrWhiteSpace(Url))
		{
			if (Space is not null && SpacePage is not null)
				return String.Format(TfConstants.SpacePagePageUrl, Space.Id, SpacePage.Id);

			return "#";
		}

		return Url.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, Id.ToString());
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

public enum TfBookmarkType
{
	[Description("URL")] URL = 0,
	[Description("Page")] Page = 1,
	[Description("Rows")] DataProviderRows = 2
}