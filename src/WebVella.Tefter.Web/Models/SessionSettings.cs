using System.Text.Json.Serialization;

namespace WebVella.Tefter.Web.Models;

public record SessionSettings
{

    [JsonPropertyName("sidebarExpanded")]
    public bool SidebarExpanded { get; init; } = true;
}
