namespace WebVella.Tefter.Demo.Models;

public record StateActiveSpaceDataChangedEventArgs
{
	public Space Space { get; set; }
	public SpaceData SpaceData { get; set; }
	public SpaceView SpaceView { get; set; }
}

public record StateActiveUserChangedEventArgs
{
	public User User { get; set; }
}

public record StateUISettingsChangedEventArgs { }

public record StateFilterChangedEventArgs { 
	public Filter Filter { get; set; } = new();
}