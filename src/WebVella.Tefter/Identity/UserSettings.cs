namespace WebVella.Tefter.Identity;

public record UserSettings
{
	public string UiTheme { get; set; }

	public string UiColor { get; set; }

	public bool IsSidebarOpen { get; set; }
}
