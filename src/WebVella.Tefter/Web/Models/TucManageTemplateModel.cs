using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucManageTemplateModel
{
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string Description { get; set; }
	[Required]
	public string FluentIconName { get; set; }
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public string SettingsJson { get; set; } = "{}";
	public Type ContentProcessorType { get; set; }
	public Guid? UserId { get; set; }
	public TucManageTemplateModel() { }
	public TucManageTemplateModel(TfManageTemplateModel model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		FluentIconName = model.FluentIconName;
		IsEnabled = model.IsEnabled;
		IsSelectable = model.IsSelectable;
		SettingsJson = model.SettingsJson;
		ContentProcessorType = model.ContentProcessorType;
		UserId = model.UserId;
	}
	public TfManageTemplateModel ToModel()
	{
		return new TfManageTemplateModel
		{
			Id = Id,
			Name = Name,
			Description = Description,
			FluentIconName = FluentIconName,
			IsEnabled = IsEnabled,
			IsSelectable = IsSelectable,
			SettingsJson = SettingsJson,
			ContentProcessorType = ContentProcessorType,
			UserId = UserId
		};
	}

}
