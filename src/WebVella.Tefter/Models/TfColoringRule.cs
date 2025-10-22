namespace WebVella.Tefter.Models;

public record TfColoringRule
{
	[JsonIgnore]
	public Guid Id { get; set; } = Guid.NewGuid();
	
	//we apply all rules that match in their order, position should be unique
	public int Position { get; set; }
	//if no columns are defined it is applicable for the whole row
	public List<string> Columns { get; set; } = new();
	public TfColor? ForegroundColor { get; set; } = null;
	public TfColor? BackgroundColor { get; set; } = null;
	//no filters match all
	public List<TfFilterQuery> Filters { get; set; } = new(); 

}
