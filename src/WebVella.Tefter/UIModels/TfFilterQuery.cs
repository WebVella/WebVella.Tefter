namespace WebVella.Tefter.Models;

public record TfFilterQuery
{
	//for calculation purposes
	[JsonIgnore]
	public Guid Id { get; set; } = Guid.NewGuid();	
	
	[JsonPropertyName("n")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("v")]
	public string? Value { get; set; }

	[JsonPropertyName("m")]
	public int Method { get; set; }

	[JsonPropertyName("i")]
	public List<TfFilterQuery> Items { get; set; } = new();

	[JsonIgnore]
	public TfFilterQuery? Parent { get; set; } = null;

	[JsonIgnore]
	public List<string> Path
	{
		get
		{
			if (Parent is null) return new List<string>() { Name };
			var parentPath = Parent.Path.ToList();
			parentPath.Add(Name);
			return parentPath;
		}
	}
}

