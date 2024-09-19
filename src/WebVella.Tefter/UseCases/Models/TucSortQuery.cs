namespace WebVella.Tefter.UseCases.Models;

public record TucSortQuery
{

	[JsonPropertyName("n")]
	public string Name { get; set; }

	[JsonPropertyName("d")]
	public int Direction { get; set; }

}

