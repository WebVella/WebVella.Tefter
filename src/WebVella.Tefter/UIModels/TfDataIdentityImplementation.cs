namespace WebVella.Tefter.Models;

public record TfDataIdentityImplementation
{
	public Guid Id { get; set; }
	public string DataProvider { get; set; } = null!;

}
