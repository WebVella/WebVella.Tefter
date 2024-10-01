namespace WebVella.Tefter.Web.Models;

public record TucRole
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public bool IsSystem { get; init; }

	public TucRole() { }
	public TucRole(Role model)
	{
		Id = model.Id;
		Name = model.Name;
		IsSystem = model.IsSystem;
	}
	public Role ToModel()
	{
		return new Role
		{
			Id = Id,
			Name = Name,
			IsSystem = IsSystem
		};
	}

}
