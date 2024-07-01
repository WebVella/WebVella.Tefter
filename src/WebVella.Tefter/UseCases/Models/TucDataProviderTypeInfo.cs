namespace WebVella.Tefter.UseCases.Models;
public record TucDataProviderTypeInfo
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string IconBase64 { get; init; }
	public Type SettingsComponentType { get; init; }
	public List<string> SupportedSourceDataTypes { get; init; }
	public TucDataProviderTypeInfo() { }
	public TucDataProviderTypeInfo(ITfDataProviderType model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		IconBase64 = model.IconBase64;
		SettingsComponentType = model.SettingsComponentType;
		SupportedSourceDataTypes = model.GetSupportedSourceDataTypes().ToList();
	}
	public ITfDataProviderType ToModel(ReadOnlyCollection<ITfDataProviderType> type)
	{
		return type.Single(x=> x.Id == Id);
	}
}
