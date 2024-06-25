using Bogus;

namespace WebVella.Tefter.Web.Models;

public record Space
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public int Position { get; init; }
	public bool IsPrivate { get; init; }
	public List<SpaceData> Data { get; init; }
	public List<Permission> Permissions { get; init; }
	public Icon Icon { get; init; }
	public OfficeColor Color { get; init; }
	public SpaceData GetActiveData(Guid? dataId)
	{
		SpaceData result = null;
		if (dataId is null) result = Data.Count > 0 ? Data[0] : null;
		else result = Data.FirstOrDefault(x => x.Id == dataId.Value);

		return result;
	}



	[JsonIgnore]
	public Action OnSelect { get; set; }

	//Faker
	public static Faker<Space> GetFaker()
	{
		var colors = Enum.GetValues<OfficeColor>();
		var icons = Icons.GetAllIcons();
		var faker = new Faker<Space>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.Name, (f, m) => f.Lorem.Sentence(3))
		.RuleFor(m => m.IsPrivate, (f, m) => f.Random.Bool())
		.RuleFor(m => m.Color, (f, m) => f.PickRandom(colors))
		.RuleFor(m => m.Icon, (f, m) => f.PickRandom(icons))
		;

		return faker;
	}
}

public class SpaceBuilder{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int Position { get; set; }
	public bool IsPrivate { get; set; }
	public List<SpaceData> DataItems { get; set; } = new();
	public List<Permission> Permissions { get; set; } = new();
	public Icon Icon { get; set; }
	public OfficeColor IconColor { get; set; }

	public SpaceBuilder(Space space)
	{
		Id = space.Id;
		Name = space.Name;
		Position = space.Position;
		IsPrivate = space.IsPrivate;
		DataItems = space.Data ?? new();
		Permissions = space.Permissions ?? new();
		Icon = space.Icon;
		IconColor = space.Color;
		
	}

	public Space Buid(){ 
		return new Space{ 
			Id = Id,
			Name = Name,
			Position = Position,
			IsPrivate = IsPrivate,
			Data = DataItems,
			Permissions = Permissions,
			Icon = Icon,
			Color = IconColor,
		};
	}
}