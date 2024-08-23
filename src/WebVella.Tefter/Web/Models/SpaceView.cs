using Bogus;

namespace WebVella.Tefter.Web.Models;

public record SpaceView
{
	public Guid Id { get; init; }
	public Guid SpaceId { get; init; }
	public Guid SpaceDataId { get; init; }
	public string Name { get; init; }
	public int Position { get; init; }
	public SpaceViewType Type { get; init; } = SpaceViewType.Report;
	public bool IsBookmarked { get; init; } = false;
	public string SpaceName { get; init; }
	public string SpaceDataName { get; init; }
	public SpaceViewMeta Meta { get; init; }

	//Getters
	public Icon Icon
	{
		get
		{
			switch (Type)
			{
				case SpaceViewType.Report: return TfConstants.GetIcon("Table");
				case SpaceViewType.Dashboard: return TfConstants.GetIcon("Board");
				case SpaceViewType.Chart: return TfConstants.GetIcon("ChartMultiple");
				case SpaceViewType.Form: return TfConstants.GetIcon("Form");

				default: return TfConstants.GetIcon("Table");
			}
		}
	}
	public Icon BookmarkIcon
	{
		get
		{
			if (IsBookmarked) return TfConstants.BookmarkONIcon;
			return TfConstants.BookmarkOFFIcon;
		}
	}

	[JsonIgnore]
	public Action OnNavigate { get; set; }

	//Faker
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
		.RuleFor(m => m.Name, (f, m) =>
		{
			var lower = String.Join(" ", f.Lorem.Words(2)).ToLowerInvariant();
			return string.Concat(lower[0].ToString().ToUpper(), lower.AsSpan(1));
		})
		.RuleFor(m => m.Type, (f, m) => f.PickRandom(itemTypes))
		.RuleFor(m => m.IsBookmarked, (f, m) => f.Random.Bool())
		;

		return faker;
	}
}

public class SpaceViewBuilder
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public Guid SpaceDataId { get; set; }
	public string Name { get; set; }
	public int Position { get; set; }
	public SpaceViewType Type { get; set; } = SpaceViewType.Report;
	public bool IsBookmarked { get; set; } = false;
	public string SpaceName { get; set; }
	public string SpaceDataName { get; set; }
	public SpaceViewMeta Meta { get; set; } = new();

	public SpaceViewBuilder(SpaceView spaceView)
	{
		Id = spaceView.Id;
		SpaceId = spaceView.SpaceId;
		SpaceDataId = spaceView.SpaceDataId;
		Name = spaceView.Name;
		Position = spaceView.Position;
		Type = spaceView.Type;
		IsBookmarked = spaceView.IsBookmarked;
		SpaceName = spaceView.SpaceName;
		SpaceDataName = spaceView.SpaceDataName;
		Meta = spaceView.Meta;
	}

	public SpaceView Buid()
	{
		return new SpaceView
		{
			Id = Id,
			SpaceId = SpaceId,
			SpaceDataId = SpaceDataId,
			Name = Name,
			Position = Position,
			Type = Type,
			IsBookmarked = IsBookmarked,
			SpaceName = SpaceName,
			SpaceDataName = SpaceDataName,
			Meta = Meta,
		};

	}
}
