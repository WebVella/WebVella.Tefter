using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfDataProviderUpdateSyncForm
{
	public Guid Id { get; set; }
	[Required]
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool SynchScheduleEnabled { get; set; } = false;

}
