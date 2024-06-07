using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class Space
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int Position { get; set; }
	public bool IsPrivate { get; set; }

	public List<SpaceDataset> DataItems { get; set; } = new();

	public UserPermissions Permissions { get; set; } = new(); //what the current user can do in the space


	//Get only
	public Color IconColor
	{
		get
		{
			if (IsPrivate) return Color.Error;
			return Color.Success;
		}
	}

	public Icon Icon
	{
		get
		{
			if (IsPrivate) return new Icons.Regular.Size20.LockClosed();
			return new Icons.Regular.Size20.LockOpen();
		}
	}

	//Actions
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
