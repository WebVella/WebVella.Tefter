using Bogus;
namespace WebVella.Tefter.Web.Models;

public record User
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
    public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
	public bool SidebarExpanded { get; init; } = true;
	[JsonIgnore]
	public string Initials
    {
        get
        {
            var list = new List<string>();
            if (!String.IsNullOrWhiteSpace(FirstName))
            {
                list.Add(FirstName.Substring(0, 1));
            }
            if (!String.IsNullOrWhiteSpace(LastName))
            {
                list.Add(LastName.Substring(0, 1));
            }

            if (list.Count == 0) return "?";

            return String.Join("", list).ToUpperInvariant();
        }
    }
	public static Faker<User> GetFaker()
    {
        var themeModes = new List<DesignThemeModes>();
        var themeColors = new List<OfficeColor>();

        foreach (DesignThemeModes item in Enum.GetValues(typeof(DesignThemeModes)))
            themeModes.Add(item);

        foreach (OfficeColor item in Enum.GetValues(typeof(OfficeColor)))
            themeColors.Add(item);

        
        var faker = new Faker<User>()
        .RuleFor(m => m.Id, (f, m) => new Guid("ca6b3f9f-3e6a-47c1-a84d-cc946c413ef8"))
        .RuleFor(m => m.FirstName, (f, m) => f.Person.FirstName)
        .RuleFor(m => m.LastName, (f, m) => f.Person.LastName)
        .RuleFor(m => m.Email, (f, m) => f.Internet.Email())
        //.RuleFor(m => m.ThemeMode, (f, m) => f.PickRandom(themeModes))
        .RuleFor(m => m.ThemeMode, (f, m) => DesignThemeModes.System)
        //.RuleFor(m => m.ThemeColor, (f, m) => f.PickRandom(themeColors))
        .RuleFor(m => m.ThemeColor, (f, m) => OfficeColor.Excel)
        ;

        return faker;
    }
}
