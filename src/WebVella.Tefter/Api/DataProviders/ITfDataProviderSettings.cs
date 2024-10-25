namespace WebVella.Tefter;

public interface ITfDataProviderSettings
{
	TfComponentMode DisplayMode { get; set; }
	string Value { get; set; }
	List<ValidationError> Validate();
}
