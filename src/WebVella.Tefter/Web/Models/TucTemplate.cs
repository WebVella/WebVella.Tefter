using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucTemplate
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string FluentIconName { get; set; }
	public List<string> UsedColumns { get; set; } = new();
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public TfTemplateResultType ResultType { get; set; }
	public string SettingsJson { get; set; } = "{}";
	public Type ContentProcessorType { get; set; }
	public DateTime CreatedOn { get; set; }
	public TucUser CreatedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
	public TucUser ModifiedBy { get; set; }
	public TucTemplate() { }
	public TucTemplate(TfTemplate model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		FluentIconName = model.FluentIconName;
		UsedColumns = model.UsedColumns;
		IsEnabled = model.IsEnabled;
		IsSelectable = model.IsSelectable;
		ResultType = model.ResultType;
		SettingsJson = model.SettingsJson;
		ContentProcessorType = model.ContentProcessorType;
		CreatedOn = model.CreatedOn;
		ModifiedOn = model.ModifiedOn;
		CreatedBy = model.CreatedBy is null ? null : new TucUser(model.CreatedBy);
		ModifiedBy = model.ModifiedBy is null ? null : new TucUser(model.ModifiedBy);
	}

}
