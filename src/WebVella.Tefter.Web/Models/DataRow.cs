namespace WebVella.Tefter.Web.Models;

public record DataRow
{
	public Guid Id { get; init; }
	public string StringId { get; init; }
	public DateTime CreatedOn { get; init; } //when the DS was created, app columns not included
	public DateTime UpdatedOn { get; init; } //when the DS was updated, app columns not included
	public Dictionary<string, DataField> Fields { get; init; } = new();

	[JsonIgnore]
	public Action<(string, object)> OnCellDataChange { get; init; }
}
