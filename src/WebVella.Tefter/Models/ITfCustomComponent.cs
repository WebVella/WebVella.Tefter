namespace WebVella.Tefter.Models;

public interface ITfCustomComponent
{
	TfComponentMode DisplayMode { get; set; }
	object Context { get; set; }
	string Value { get; set; }
	EventCallback<string> ValueChanged { get; set; }
	List<ValidationError> Validate();
}
