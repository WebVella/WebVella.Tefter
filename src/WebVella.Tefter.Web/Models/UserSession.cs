namespace WebVella.Tefter.Web.Models;

public record UserSession
{
	public Space Space { get; init; }
	public SpaceData SpaceData { get; init; }
	public SpaceView SpaceView { get; init; }
}
