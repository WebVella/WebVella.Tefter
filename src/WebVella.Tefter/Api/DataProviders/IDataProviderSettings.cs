namespace WebVella.Tefter;

public interface IDataProviderSettings
{
	string Value { get; set; }
	List<ValidationError> Validate();
}
