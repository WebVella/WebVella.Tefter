namespace WebVella.Tefter.UseCases.Models;

public record TucDataProvider
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public int Index { get; init; }
	public string SettingsJson { get; init; }
	public List<TucDataProviderColumn> Columns { get; init; }
	public List<TucDataProviderSharedKey> SharedKeys { get; init; }
	public TucDataProviderTypeInfo ProviderType { get; init; }

	public TucDataProvider() { }
	public TucDataProvider(TfDataProvider model)
	{
		Id = model.Id;
		Name = model.Name;
		Index = model.Index;
		SettingsJson = model.SettingsJson;
		Columns = model.Columns is null ? null : model.Columns.Select(x=> new TucDataProviderColumn(x)).ToList();
		SharedKeys = model.SharedKeys is null ? null : model.SharedKeys.Select(x=> new TucDataProviderSharedKey(x)).ToList();
		ProviderType = new TucDataProviderTypeInfo(model.ProviderType);
	}
	public TfDataProvider ToModel(ReadOnlyCollection<ITfDataProviderType> providerTypes)
	{
		return new TfDataProvider
		{
			Id = Id,
			Name = Name,
			Index = Index,
			SettingsJson = SettingsJson,
			Columns = Columns is null ? null : Columns.Select(x=> x.ToModel()).ToList().AsReadOnly(),
			SharedKeys = SharedKeys is null ? null : SharedKeys.Select(x=> x.ToModel()).ToList().AsReadOnly(),
			ProviderType = ProviderType.ToModel(providerTypes),
		};
	}

}
