using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucRole
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	//Should not be managed by the user
	public bool IsSystem { get; set; } = false;

	public TucRole() { }
	public TucRole(TfRole model)
	{
		Id = model.Id;
		Name = model.Name;
		IsSystem = model.IsSystem;
	}
	public TfRole ToModel()
	{
		return new TfRole
		{
			Id = Id,
			Name = Name,
			IsSystem = IsSystem
		};
	}

}
