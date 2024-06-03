using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class ThemeLocalStorage
{
	[JsonPropertyName("mode")]
	public string Mode { get; set; }

	[JsonPropertyName("primaryColor")]
	public string PrimaryColor { get; set; }
}

