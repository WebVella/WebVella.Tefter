using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfTemplate
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string FluentIconName { get; set; }
	public List<Guid> SpaceDataList { get; set; } = new();
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public TfTemplateResultType ResultType { get; set; }
	public string? SettingsJson { get; set; }
	public Type ContentProcessorType { get; set; }
	public DateTime CreatedOn { get; set; }
	public TfUser CreatedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
	public TfUser ModifiedBy { get; set; }
}

public record TfManageTemplateModel
{
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string Description { get; set; }
	[Required]
	public string FluentIconName { get; set; }
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public string? SettingsJson { get; set; }
	public Type ContentProcessorType { get; set; }
	public Guid? UserId { get; set; }
	public List<Guid> SpaceDataList { get; set; } = new();
}
