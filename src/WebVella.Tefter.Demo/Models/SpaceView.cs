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
	public string SpaceName { get; set; }
	public string SpaceDataName { get; set; }
	public SpaceViewMeta Meta { get; set; } = new();

	//Getters
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

public class SpaceViewMeta
{
	public List<SpaceViewColumn> Columns { get; set; } = new();
}

public class SpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }
	public Guid? AppId { get; set; } // null for system
	public string AppColumnName { get; set; } // for query and as dev name
	public string Title { get; set; }
	public int Position { get; set; } = 1;
	public DateTime CreatedOn { get; set; }
	public User CreatedBy { get; set; } = null; //null if system
	public DateTime UpdatedOn { get; set; }
	public User UpdatedBy { get; set; }
	public ColumnType DataType { get; set; } = ColumnType.Text;
	public object Settings { get; set; } //Based on type -> dropdown can have the values here
	public bool IsVisible { get; set; } = true;
	public string? Width { get; set; } = null;

	public string CellComponent { get; set; }
	public string CellAssembly { get; set; }

	public string CellTypeString
	{
		get
		{
			if (String.IsNullOrWhiteSpace(CellComponent)) return null;
			if (!String.IsNullOrWhiteSpace(CellAssembly))
				return $"{CellComponent}, {CellAssembly}";
			else
				return CellComponent;
		}
	}

	public Type CellType
	{
		get
		{
			if (String.IsNullOrWhiteSpace(CellTypeString)) return null;
			return Type.GetType(CellTypeString);
		}
	}

	public object CellProperties { get; set; }

	public IDictionary<string, object> GetComponentParameters(DataRow context)
	{
		var result = new Dictionary<string, object>();
		result["Data"] = context.Fields.ContainsKey(AppColumnName) ? context.Fields[AppColumnName] : null;
		result["Meta"] = this;
		result["ValueChanged"] = context.OnCellDataChange;
		return result;
	}

}

public enum ColumnType
{
	File,
	Numeric,
	Date,
	LongText,
	MultiplePerson,
	Name,
	Person,
	Color,
	SubRecords,
	Team,
	Text,
	Dropdown,
	Integration,
	Group,
	Email,
	Lookup,
	Hour,
	ColorPicker,
	TimeRange,
	Timezone,
	Country,
	Location,
	Link,
	Week,
	Votes,
	CustomUrl,
	Action,
	Rating,
	Boolean,
	Autonumber,
	Phone,
	Document,
	Button,
	Dependency,
	Space,
	Empty,
	Currency
}