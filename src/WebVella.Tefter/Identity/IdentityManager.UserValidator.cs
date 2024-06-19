namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
}

public partial class IdentityManager : IIdentityManager
{
	internal class UserValidator : AbstractValidator<User>
	{
		public UserValidator(IDboManager dboManager, IIdentityManager identityManager)
		{
			RuleSet("general", () =>
			{
				RuleFor(user => user.Email)
					.NotEmpty()
					.WithMessage("The email is required.");

				RuleFor(user => user.Email)
					.Must(email => { return email.IsEmail(); })
					.WithMessage("The email is incorrect.");

				RuleFor(user => user.FirstName)
					.NotEmpty()
					.WithMessage("The first name is required.");

				RuleFor(user => user.LastName)
					.NotEmpty()
					.WithMessage("The last name is required.");
				
				RuleFor(user => user.Password)
					.NotEmpty()
					.WithMessage("The password is required.");

			});

			RuleSet("create", () =>
			{
				RuleFor(user => user.Id)
					.Must(id => { return identityManager.GetUser(id).Value == null; })
					.WithMessage("There is already existing user with specified id.");

				RuleFor(user => user.Email)
					.Must(email => { return identityManager.GetUser(email).Value == null; })
					.WithMessage("There is already existing user with specified email.");
			});

			RuleSet("update", () =>
			{
				RuleFor(user => user.Id)
					.NotEmpty()
					.WithMessage("User identifier is not provided.");

				RuleFor(user => user.Id)
					.Must( id => { return identityManager.GetUser(id).Value != null; })
					.WithMessage("There is no existing user for specified identifier.");

				RuleFor(user => user.Email)
					.Must((user, email) =>
					{
						var existingUser = identityManager.GetUser(user.Email).Value;
						return !(existingUser != null && existingUser.Id != user.Id);
					})
					.WithMessage("There is already existing user with specified email.");
			});
		}

		public ValidationResult ValidateCreate(User user)
		{
			return this.Validate(user, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(User user)
		{
			return this.Validate(user, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}
	}
}
