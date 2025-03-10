namespace WebVella.Tefter.Web.Models;

public record TucRole
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public bool IsSystem { get; init; }

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
