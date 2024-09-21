using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucSearchResult
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public TucSearchResultType Type { get; set; } = TucSearchResultType.Space;
	public DateTime? CreatedOn { get; set; }
	public Guid? SpaceViewId { get; set; }
	public Guid? SpaceId { get; set; }
	public string SpaceViewName { get; set; }
	public string SpaceName { get; set; }
	public OfficeColor SpaceColor { get; set; } = OfficeColor.Default;
	public Icon SpaceIcon { get; set; } = null;
	public string Url { get; set; }
	public TucSearchResult() { }
	public TucSearchResult(TucBookmark model,
		TucSearchResultType type)
	{
		Id = model.Id;
		Title = model.Name;
		Description = model.Description;
		Type = type;
		CreatedOn = model.CreatedOn;
		SpaceViewId = model.SpaceViewId;
		SpaceId = model.SpaceId;
		SpaceName = model.SpaceName;
		SpaceViewName = model.SpaceViewName;
		SpaceColor = model.SpaceColor;
		SpaceIcon = model.SpaceIcon;
		if (type == TucSearchResultType.Bookmark)
			Url = String.Format(TfConstants.SpaceViewPageUrl, model.SpaceId, model.SpaceViewId);
		else if (type == TucSearchResultType.UrlSave)
		{
			Url = NavigatorExt.AddQueryValueToUri(model.Url, TfConstants.ActiveSaveQueryName,model.Id.ToString());
		}
	}

	public TucSearchResult(TucSpaceView model, TucSpace space)
	{
		Id = model.Id;
		Title = model.Name;
		Description = null;
		Type = TucSearchResultType.SpaceView;
		CreatedOn = null;
		SpaceViewId = model.Id;
		SpaceId = model.SpaceId;
		SpaceName = space.Name;
		SpaceViewName = model.Name;
		SpaceColor = space.Color;
		SpaceIcon = space.Icon;
		Url = String.Format(TfConstants.SpaceViewPageUrl, model.SpaceId, model.Id);
	}

	public TucSearchResult(TucSpace model)
	{
		Id = model.Id;
		Title = model.Name;
		Description = null;
		Type = TucSearchResultType.Space;
		CreatedOn = null;
		SpaceViewId = null;
		SpaceId = model.Id;
		SpaceName = model.Name;
		SpaceViewName = model.Name;
		SpaceColor = model.Color;
		SpaceIcon = model.Icon;
		Url = String.Format(TfConstants.SpacePageUrl, model.Id);
	}
}

public enum TucSearchResultType
{
	[Description("space")]
	Space = 0,
	[Description("view")]
	SpaceView = 1,
	[Description("bookmark")]
	Bookmark = 2,
	[Description("saved url")]
	UrlSave = 3
}