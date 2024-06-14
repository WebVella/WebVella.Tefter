namespace WebVella.Tefter.Web.Models;

public record Permission
{
	public Guid Id { get; init; }
	public bool Can { get; init; }
	public string Reason { get; init; }
}
