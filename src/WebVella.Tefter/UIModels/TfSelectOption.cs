namespace WebVella.Tefter.Models;

public record TfSelectOption
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public object? Value { get; set; }
	public string? Label { get; set; }
	public string? IconName { get; set; }
	public string? Color { get; set; }
	public string? BackgroundColor { get; set; }
	public bool HideLabel { get; set; } = false;

	[JsonIgnore]
	public Action OnClick { get; set; }
	
	[JsonIgnore]
	public Action OnBookmark { get; set; }	

	public TfSelectOption() { }
	public TfSelectOption(
		object? value, 
		string? label = null,
		string? iconName = null,
		string? color = null,
		string? backgroundColor = null,
		bool hideLabel = false)
	{
		Value = value;
		Label = label;
		IconName = iconName;
		Color = color;
		BackgroundColor = backgroundColor;
		HideLabel = hideLabel;
	}
}
