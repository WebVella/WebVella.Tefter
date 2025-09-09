namespace WebVella.Tefter.Models;

public record TfSortQuery
{
	[JsonPropertyName("n")]
	public string Name { get; set; } = default!;

	[JsonPropertyName("d")]
	public int Direction { get; set; } = (int)TfSortDirection.ASC;

}

