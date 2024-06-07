using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class DataRow
{
	public Guid Id { get; set; }
	public string StringId { get; set; }
	public DateTime CreatedOn { get; set; } //when the DS was created, app columns not included
	public DateTime UpdatedOn { get; set; } //when the DS was updated, app columns not included
	public Dictionary<string, DataField> Fields { get; set; } = new();

	[JsonIgnore]
	public Action<(string,object)> OnCellDataChange { get; set; }

}
