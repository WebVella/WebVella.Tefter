using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class SpaceItemView
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int Position { get; set; }

	[JsonIgnore]
	public Action OnNavigate { get; set; }

	public static Faker<SpaceItemView> GetFaker()
	{
		var faker = new Faker<SpaceItemView>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.Name, (f, m) => {
			var lower = String.Join(" ", f.Lorem.Words(2)).ToLowerInvariant();
			return string.Concat(lower[0].ToString().ToUpper(), lower.AsSpan(1));
		 })
		;

		return faker;
	}
}
