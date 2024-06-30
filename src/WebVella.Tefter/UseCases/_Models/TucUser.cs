namespace WebVella.Tefter.UseCases.Models;

public record TucUser
{
	public Guid Id { get; init; }
	public string Email { get; init; }
	public string FirstName { get; init; }
	public string LastName { get; init; }
	public string Password { get; init; }
	public bool Enabled { get; init; }
	public DateTime CreatedOn { get; init; }
	public TucUserSettings Settings { get; init; } = new();
	public ReadOnlyCollection<TucRole> Roles { get; init; }
	public TucUser() { }

	public TucUser(User model)
	{
		Id = model.Id;
		Email = model.Email;
		FirstName = model.FirstName;
		LastName = model.LastName;
		Password = model.Password;
		Enabled = model.Enabled;
		CreatedOn = model.CreatedOn;
		Settings = new TucUserSettings(model.Settings);
		Roles = model.Roles.Select(x => new TucRole(x)).ToList().AsReadOnly();
	}

	public User ToModel()
	{
		return new User
		{
			Id = Id,
			Email = Email,
			FirstName = FirstName,
			LastName = LastName,
			Password = Password,
			Enabled = Enabled,
			CreatedOn = CreatedOn,
			Settings = Settings.ToModel(),
			Roles = Roles.Select(x=> x.ToModel()).ToList().AsReadOnly()
		};
	}
}
