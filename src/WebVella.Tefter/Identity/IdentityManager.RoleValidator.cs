namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
}

public partial class IdentityManager : IIdentityManager
{
	internal class RoleValidator : AbstractValidator<Role>
	{
		public RoleValidator(ITfDboManager dboManager, IIdentityManager identityManager)
		{
			RuleSet("general", () =>
			{
				RuleFor(role => role.Name)
					.NotEmpty()
					.WithMessage("Role name is required.");
			});

			RuleSet("create", () =>
			{
				RuleFor(role => role.Name)
					.Must(name => { return identityManager.GetRole(name) == null; })
					.WithMessage("There is already existing role with specified name.");

				RuleFor(role => role.Id)
					.Must(id => { return identityManager.GetRole(id) == null; })
					.WithMessage("There is already existing role with specified identifier.");
			});

			RuleSet("update", () =>
			{
				RuleFor(role => role.Id)
					.Must((role, id) => { return identityManager.GetRole(id) != null; })
					.WithMessage("There is no existing role for specified identifier.");

				RuleFor(role => role.Name)
					.Must((role, name) =>
					{
						var existingRole = identityManager.GetRole(role.Name);
						return !(existingRole != null && existingRole.Id != role.Id);
					})
					.WithMessage("There is already existing role with specified name.");
			});

			RuleSet("delete", () =>
			{
				RuleFor(role => role.Id)
					.Must((role, id) => { return identityManager.GetRole(id) != null; })
					.WithMessage("There is no existing role for specified identifier.");

				RuleFor(role => role.IsSystem)
					.Must((role, isSystem ) => { return !role.IsSystem; })
					.WithMessage("The role is system and cannot be deleted.");

				RuleFor(role => role.Id)
					.Must(id => 
					{
						var exists = dboManager.ExistsAny<UserRoleDbo>(" role_id = @role_id ", new NpgsqlParameter("@role_id", id));
						return !exists;
					})
					.WithMessage("There is one or more existing users with this role.");
			});
		}

		public ValidationResult ValidateCreate(Role role)
		{
			if (role == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The role instance is null.") });

			return this.Validate(role, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(Role role)
		{
			if (role == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The role instance is null.") });

			return this.Validate(role, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(Role role)
		{
			if (role == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The role instance is null.") });

			return this.Validate(role, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}
}
