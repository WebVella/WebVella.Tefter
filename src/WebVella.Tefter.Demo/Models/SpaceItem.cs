using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class SpaceItem
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public int Position { get; set; }
	public SpaceItemType ItemType { get; set; } = SpaceItemType.Report;

	public Icon Icon
	{
		get
		{
			switch(ItemType){ 
				case SpaceItemType.Report: return new Icons.Regular.Size20.Table();
				case SpaceItemType.Dashboard: return new Icons.Regular.Size20.Board();
				case SpaceItemType.Chart: return new Icons.Regular.Size20.ChartMultiple();
				case SpaceItemType.Form: return new Icons.Regular.Size20.Form();

				default: return new Icons.Regular.Size20.Table();
			}
		}
	}

	[JsonIgnore]
	public Action OnSelect { get; set; }

	public static Faker<SpaceItem> GetFaker()
	{
		var itemTypes = new List<SpaceItemType>(){
			SpaceItemType.Report,
			SpaceItemType.Dashboard,
			SpaceItemType.Chart,
			SpaceItemType.Form,
		};	
		var faker = new Faker<SpaceItem>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.Name, (f, m) => f.Lorem.Sentence(3))
		.RuleFor(m => m.ItemType, (f, m) => f.PickRandom(itemTypes))
		;

		return faker;
	}
}
