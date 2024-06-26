namespace WebVella.Tefter;

public interface ITfDataProviderSettings
{
	ComponentDisplayMode DisplayMode { get; set; }
	string Value { get; set; }
	List<ValidationError> Validate();
}
