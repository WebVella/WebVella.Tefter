namespace WebVella.Tefter.UseCases.Models;

public record TucSelectOption
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public object Value { get; set; }
	public string Label { get; set; }
	[JsonIgnore]
	public Action OnClick { get; set; }

	public TucSelectOption(){}
	public TucSelectOption(object value, string label){
		Value = value;
		Label = label;
	}
}
