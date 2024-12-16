using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Templates.Models;

public class Template
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Icon { get; set; }
	public List<string> UsedColumns { get; set; } = new();
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public TemplateResultType ResultType { get; set; }
	public string SettingsJson { get; set; }
	public Type ContentProcessorType { get; set; }
	public DateTime CreatedOn { get; set; }
	public User CreatedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
	public User ModifiedBy { get; set; }
}

public class CreateTemplateModel
{
	[Required]
	public string Name { get; set; }
	public string Description { get; set; }
	[Required]
	public string Icon { get; set; }
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public string SettingsJson { get; set; }
	public Type ContentProcessorType { get; set; }
	public Guid? UserId { get; set; }	
}

public class UpdateTemplateModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Icon { get; set; }
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public TemplateResultType ResultType { get; set; }
	public string SettingsJson { get; set; }
	public Type ContentProcessorType { get; set; }
	public Guid? UserId { get; set; }
}