using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucBookmark
{
	public Guid Id { get; set; }
	[Required(ErrorMessage = "required")]
	public string Name { get; set; }
	public string Description { get; set; }
	public string Url { get; set; }
	public DateTime CreatedOn { get; set; }
	public Guid UserId { get; set; }
	public Guid SpaceViewId { get; set; }
	public string SpaceViewName { get; set; }
	public string SpaceName { get; set; }
	public OfficeColor SpaceColor { get; set; } = OfficeColor.Default;
	public Icon SpaceIcon { get; set; } = null;
	public Guid SpaceId { get; set; }
	public List<TucTag> Tags { get; set; } = new();

	public TucBookmark() { }
	public TucBookmark(TfBookmark model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		Url = model.Url;
		CreatedOn = model.CreatedOn;
		UserId = model.UserId;
		SpaceViewId = model.SpaceViewId;
		Tags = model.Tags != null ? model.Tags.Select(x => new TucTag(x)).ToList() : new();
	}
	public TfBookmark ToModel()
	{
		return new TfBookmark
		{
			Id = Id,
			Name = Name,
			Description = Description,
			Url = Url,
			CreatedOn = CreatedOn,
			UserId = UserId,
			SpaceViewId = SpaceViewId,
			Tags = Tags != null ? Tags.Select(x => x.ToModel()).ToList() : new()
		};
	}

}
