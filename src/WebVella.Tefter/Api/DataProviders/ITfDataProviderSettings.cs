namespace WebVella.Tefter;

public interface ITfDataProviderSettings
{
	string Value { get; set; }
	List<ValidationError> Validate();
}
