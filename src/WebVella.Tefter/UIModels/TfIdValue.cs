using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfIdValue
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }
	[JsonPropertyName("value")]
	public string? Value { get; set; }
	
}
