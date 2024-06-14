using Bogus;

namespace WebVella.Tefter.Web.Models;

public record SpaceData
{
	public Guid Id { get; init; }
	public Guid SpaceId { get; init; }
	public string Name { get; init; }
	public string SpaceName { get; init; }
	public int Position { get; init; }
	public Guid MainViewId { get; init; } //should be in Views position 0
	public List<SpaceView> Views { get; init; } = new();

	public SpaceView GetActiveView(Guid? viewId)
	{
		SpaceView result = null;
		if (viewId is null) result = Views.FirstOrDefault(x => x.Id == MainViewId);
		else result = Views.FirstOrDefault(x => x.Id == viewId.Value);

		return result;
	}


	[JsonIgnore]
	public Action OnSelect { get; set; }

	public static Faker<SpaceData> GetFaker()
	{
		var faker = new Faker<SpaceData>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.Name, (f, m) => f.Lorem.Sentence(3))
		;

		return faker;
	}
}

public class SpaceDataBuilder{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public string SpaceName { get; set; }
	public int Position { get; set; }
	public Guid MainViewId { get; set; }
	public List<SpaceView> Views { get; set; } = new();

	public SpaceDataBuilder(SpaceData spaceData)
	{
		Id = spaceData.Id;
		SpaceId = spaceData.SpaceId;
		Name = spaceData.Name;
		SpaceName = spaceData.SpaceName;
		Position = spaceData.Position;
		MainViewId = spaceData.MainViewId;
		Views = spaceData.Views ?? new();
	}

	public SpaceData Build(){ 
		return new SpaceData{ 
			Id = Id, 
			SpaceId = SpaceId, 
			Name = Name, 
			SpaceName = SpaceName, 
			Position = Position,
			MainViewId = MainViewId,
			Views = Views
		};
	}
}
