using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class SpaceView
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public Guid SpaceDataId { get; set; }
	public string Name { get; set; }
	public int Position { get; set; }
	public SpaceViewType Type { get; set; } = SpaceViewType.Report;

	public bool IsBookmarked { get; set; } = false;

	public Icon Icon
	{
		get
		{
			switch (Type)
			{
				case SpaceViewType.Report: return new Icons.Regular.Size20.Table();
				case SpaceViewType.Dashboard: return new Icons.Regular.Size20.Board();
				case SpaceViewType.Chart: return new Icons.Regular.Size20.ChartMultiple();
				case SpaceViewType.Form: return new Icons.Regular.Size20.Form();

				default: return new Icons.Regular.Size20.Table();
			}
		}
	}

	public Icon BookmarkIcon
	{
		get
		{
			if (IsBookmarked) return new Icons.Filled.Size20.Star();
			return new Icons.Regular.Size20.Star();
		}
	}


	[JsonIgnore]
	public Action OnNavigate { get; set; }

	public static Faker<SpaceView> GetFaker()
	{
		var itemTypes = new List<SpaceViewType>(){
			SpaceViewType.Report,
			SpaceViewType.Dashboard,
			SpaceViewType.Chart,
			SpaceViewType.Form,
		};

		var faker = new Faker<SpaceView>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.Name, (f, m) => {
			var lower = String.Join(" ", f.Lorem.Words(2)).ToLowerInvariant();
			return string.Concat(lower[0].ToString().ToUpper(), lower.AsSpan(1));
		 })
		.RuleFor(m => m.Type, (f, m) => f.PickRandom(itemTypes))
		.RuleFor(m => m.IsBookmarked, (f, m) => f.Random.Bool())
		;

		return faker;
	}
}
