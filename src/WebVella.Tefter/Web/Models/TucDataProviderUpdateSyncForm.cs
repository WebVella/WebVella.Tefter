using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucDataProviderUpdateSyncForm
{
	public Guid Id { get; set; }
	[Required]
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool SynchScheduleEnabled { get; set; } = false;

	public TucDataProviderUpdateSyncForm() { }
	public TucDataProviderUpdateSyncForm(TfCreateDataProvider model)
	{
		Id = model.Id;
		SynchScheduleMinutes = model.SynchScheduleMinutes;
		SynchScheduleEnabled = model.SynchScheduleEnabled;
	}
	public TfCreateDataProvider ToModel(TfDataProvider currentProvider)
	{
		var result = new TfCreateDataProvider();
		if(currentProvider is not null){ 
			result = new TfCreateDataProvider{ 
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
		result.SynchScheduleMinutes = SynchScheduleMinutes;
		result.SynchScheduleEnabled = SynchScheduleEnabled;
		return result;
	}

	public TfUpdateDataProvider ToUpdateModel(TfDataProvider currentProvider)
	{
		var result = new TfUpdateDataProvider();
		if (currentProvider is not null)
		{
			result = new TfUpdateDataProvider
			{
				Id = currentProvider.Id,
				Name = currentProvider.Name,
				SettingsJson = currentProvider.SettingsJson,
				SynchPrimaryKeyColumns = currentProvider.SynchPrimaryKeyColumns.ToList(),
				SynchScheduleEnabled = currentProvider.SynchScheduleEnabled,
				SynchScheduleMinutes = currentProvider.SynchScheduleMinutes,
			};
		}

		result.Id = Id;
		result.SynchScheduleMinutes = SynchScheduleMinutes;
		result.SynchScheduleEnabled = SynchScheduleEnabled;
		return result;
	}

}
