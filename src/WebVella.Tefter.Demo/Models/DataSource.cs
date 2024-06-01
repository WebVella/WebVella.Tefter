using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class DataSource
{
	public Guid Id { get; set; }
	public string Email { get; set; }

	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Company { get; set; }
	public DateTime BirthDate { get; set; }


	public static Faker<DataSource> GetFaker()
	{
		var faker = new Faker<DataSource>()
		.RuleFor(m => m.Id, (f, m) => f.Random.Uuid())
		.RuleFor(m => m.FirstName, (f, m) => f.Name.FirstName())
		.RuleFor(m => m.LastName, (f, m) => f.Name.LastName())
		.RuleFor(m => m.Company, (f, m) => f.Company.CompanyName())
		.RuleFor(m => m.BirthDate, (f, m) => f.Person.DateOfBirth.Date)
		;

		return faker;
	}
}
