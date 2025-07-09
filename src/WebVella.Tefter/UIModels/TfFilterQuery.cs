namespace WebVella.Tefter.Models;

public record TfFilterQuery
{
	[JsonPropertyName("t")]
	public string Type { get; set; } = default!;

	[JsonPropertyName("n")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("v")]
	public string? Value { get; set; }

	[JsonPropertyName("m")]
	public int Method { get; set; }

	[JsonPropertyName("i")]
	public List<TfFilterQuery> Items { get; set; } = new();

}

