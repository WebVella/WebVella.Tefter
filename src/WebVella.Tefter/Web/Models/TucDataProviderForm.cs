﻿using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucDataProviderForm
{
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string SettingsJson { get; set; }
	[Required]
	public TucDataProviderTypeInfo ProviderType { get; set; }

	public TucDataProviderForm() { }

	public TfDataProviderModel ToModel(ReadOnlyCollection<ITfDataProviderAddon> providerTypes, TfDataProvider currentProvider)
	{
		var result = new TfDataProviderModel();
		if(currentProvider is not null){ 
			result = new TfDataProviderModel{ 
				Id = currentProvider.Id,
				Name = currentProvider.Name,
				ProviderType = currentProvider.ProviderType,
				SettingsJson = currentProvider.SettingsJson,
				SynchPrimaryKeyColumns = currentProvider.SynchPrimaryKeyColumns.ToList(),
				SynchScheduleEnabled = currentProvider.SynchScheduleEnabled,
				SynchScheduleMinutes = currentProvider.SynchScheduleMinutes,
			};
		}

		result.Id = Id;
		result.Name = Name;
		result.SettingsJson = SettingsJson;
		result.ProviderType = ProviderType.ToModel(providerTypes);

		return result;
	}

}
