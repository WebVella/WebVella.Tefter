using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucDataProviderUpdateSyncForm
{
	public Guid Id { get; set; }
	[Required]
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool SynchScheduleEnabled { get; set; } = false;

	public TucDataProviderUpdateSyncForm() { }
	public TucDataProviderUpdateSyncForm(TfDataProviderModel model)
	{
		Id = model.Id;
		SynchScheduleMinutes = model.SynchScheduleMinutes;
		SynchScheduleEnabled = model.SynchScheduleEnabled;
	}
	public TfDataProviderModel ToModel(TfDataProvider currentProvider)
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
		result.SynchScheduleMinutes = SynchScheduleMinutes;
		result.SynchScheduleEnabled = SynchScheduleEnabled;
		return result;
	}

}
