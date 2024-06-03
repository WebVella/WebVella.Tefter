using Bogus;

namespace WebVella.Tefter.Demo.Models;

public class User
{
	public Guid Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Email { get; set; }
	public DesignThemeModes ThemeMode { get; set; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; set; } = OfficeColor.Excel;

	public static Faker<User> GetFaker()
	{
		var themeModes = new List<DesignThemeModes>();
		var themeColors = new List<OfficeColor>();

		foreach (DesignThemeModes item in Enum.GetValues(typeof(DesignThemeModes)))
			themeModes.Add(item);

		foreach (OfficeColor item in Enum.GetValues(typeof(OfficeColor)))
			themeColors.Add(item);


		var faker = new Faker<User>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.FirstName, (f, m) => f.Person.FirstName)
		.RuleFor(m => m.LastName, (f, m) => f.Person.LastName)
		.RuleFor(m => m.Email, (f, m) => f.Internet.Email())
		.RuleFor(m => m.ThemeMode, (f, m) => f.PickRandom(themeModes))
		.RuleFor(m => m.ThemeColor, (f, m) => f.PickRandom(themeColors))
		;

		return faker;
	}
}
