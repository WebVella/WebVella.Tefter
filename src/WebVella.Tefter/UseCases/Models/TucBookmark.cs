namespace WebVella.Tefter.UseCases.Models;

public record TucBookmark
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string Url { get; init; }
	public DateTime CreatedOn { get; init; }
	public Guid UserId { get; init; }
	public Guid SpaceViewId { get; init; }
	public string SpaceViewName { get; init; }
	public string SpaceName { get; init; }
	public OfficeColor SpaceColor { get; set; } = OfficeColor.Default;
	public Icon SpaceIcon { get; set; } = null;
	public List<TucTag> Tags { get; init; } = new();

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
