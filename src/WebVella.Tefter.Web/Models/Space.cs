using Bogus;

namespace WebVella.Tefter.Web.Models;

public record Space
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public int Position { get; init; }
	public bool IsPrivate { get; init; }
	public List<SpaceData> DataItems { get; init; }
	public List<Permission> Permissions { get; init; }

	[JsonIgnore]
	public Color IconColor
	{
		get
		{
			if (IsPrivate) return Color.Error;
			return Color.Success;
		}
	}
	[JsonIgnore]
	public Icon Icon
	{
		get
		{
			if (IsPrivate) return new Icons.Regular.Size20.LockClosed();
			return new Icons.Regular.Size20.LockOpen();
		}
	}
	[JsonIgnore]
	public Action OnSelect { get; set; }

	//Faker
	public static Faker<Space> GetFaker()
	{
		var faker = new Faker<Space>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.Name, (f, m) => f.Lorem.Sentence(3))
		.RuleFor(m => m.IsPrivate, (f, m) => f.Random.Bool())
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

	public SpaceBuilder(Space space)
	{
		Id = space.Id;
		Name = space.Name;
		Position = space.Position;
		IsPrivate = space.IsPrivate;
		DataItems = space.DataItems ?? new();
		Permissions = space.Permissions ?? new();
	}

	public Space Buid(){ 
		return new Space{ 
			Id = Id,
			Name = Name,
			Position = Position,
			IsPrivate = IsPrivate,
			DataItems = DataItems,
			Permissions = Permissions
		};
	}
}