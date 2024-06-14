using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class SpaceData
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public string SpaceName { get; set; }
	public int Position { get; set; }
	public Guid MainViewId { get; set; } //should be in Views position 0
	public List<SpaceView> Views { get; set; } = new();

	public SpaceView GetActiveView(Guid? viewId)
	{
		SpaceView result = null;
		if (viewId is null) result = Views.FirstOrDefault(x=> x.Id == MainViewId);
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
