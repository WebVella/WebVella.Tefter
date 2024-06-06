using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class UISettings
{
	[JsonPropertyName("mode")]
	public string Mode { get; set; }

	[JsonPropertyName("primaryColor")]
	public string PrimaryColor { get; set; }

	[JsonPropertyName("sidebarExpanded")]
	public bool SidebarExpanded { get; set; } = true;
}

