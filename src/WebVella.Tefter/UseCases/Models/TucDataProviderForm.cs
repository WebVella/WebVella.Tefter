using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucDataProviderForm
{
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string SettingsJson { get; set; }
	[Required]
	public TucDataProviderTypeInfo ProviderType { get; set; }

	public TucDataProviderForm() { }
	public TucDataProviderForm(TfDataProviderModel model)
	{
		Id = model.Id;
		Name = model.Name;
		SettingsJson = model.SettingsJson;
		ProviderType = new TucDataProviderTypeInfo(model.ProviderType);
	}
	public TfDataProviderModel ToModel(ReadOnlyCollection<ITfDataProviderType> providerTypes)
	{
		return new TfDataProviderModel
		{
			Id = Id,
			Name = Name,
			SettingsJson = SettingsJson,
			ProviderType = ProviderType.ToModel(providerTypes),
		};
	}

}
