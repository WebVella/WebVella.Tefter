using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.Web.Models;

public record TucFilterQuery
{
	[JsonPropertyName("t")]
	public string Type { get; set; }

	[JsonPropertyName("n")]
	public string Name { get; set; }

	[JsonPropertyName("v")]
	public string Value { get; set; }

	[JsonPropertyName("m")]
	public int Method { get; set; }

	[JsonPropertyName("i")]
	public List<TucFilterQuery> Items { get; set; } = new();

}

