﻿namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.6.14.1")]
internal class TefterSystemMigration2024061801 : ITefterSystemMigration
{
	public void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		// TABLE: MIGRATION
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "migration")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("addon_id", c => { c.WithoutAutoDefaultValue().Nullable(); })
					.AddTextColumn("migration_class_name", c => { c.NotNullable(); })
					.AddDateTimeColumn("executed_on", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddNumberColumn("major_ver", c => { c.NotNullable(); })
					.AddNumberColumn("minor_ver", c => { c.NotNullable(); })
					.AddNumberColumn("build_ver", c => { c.NotNullable(); })
					.AddNumberColumn("revision_ver", c => { c.NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_migration_id", c => { c.WithColumns("id"); });
			});

		// TABLE: USER
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "user")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("email", c => { c.NotNullable(); })
					.AddTextColumn("password", c => { c.NotNullable(); })
					.AddTextColumn("first_name", c => { c.NotNullable().WithDefaultValue(""); })
					.AddTextColumn("last_name", c => { c.NotNullable().WithDefaultValue(""); })
					.AddTextColumn("settings_json", c => { c.NotNullable().WithDefaultValue("{}"); })
					.AddBooleanColumn("enabled", c => { c.NotNullable().WithDefaultValue(true); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_user_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_user_email", c => { c.WithColumns("email"); });
			});

		// TABLE: ROLE
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "role")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("name", c => { c.NotNullable(); })
					.AddBooleanColumn("is_system", c => { c.NotNullable().WithDefaultValue(false); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_role_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_role_name", c => { c.WithColumns("name"); });
				
			});

		// TABLE: USER_ROLE
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "user_role")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("user_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("role_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_user_role", c => { c.WithColumns("user_id", "role_id"); })
					.AddForeignKeyConstraint("fk_user_role_role", c =>
						{
							c
							.WithColumns("role_id")
							.WithForeignTable("role")
							.WithForeignColumns("id");
						})
					.AddForeignKeyConstraint("fk_user_role_user", c =>
						{
							c
							.WithColumns("user_id")
							.WithForeignTable("user")
							.WithForeignColumns("id");
						});
			});
	}

	public void MigrateData(IDatabaseService dbService, IDboManager dboManager)
	{
		/// CREATES INITIAL ADMINISTRATOR USER, ROLE AND RELATION BETWEEN
		/// 
		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "admin@tefter.bg",
			Password = "123".ToMD5Hash(),
			FirstName = "Tefter",
			LastName = "Administrator",
			CreatedOn = DateTime.Now,
			Enabled = true,
			Settings = new UserSettings()
		};

		var success = dboManager.Insert<User>(user);
		if (!success)
			throw new DatabaseException("Failed to insert initial user record.");

		var role = new Role
		{
			Id = Guid.NewGuid(),
			Name = "Administrator",
			IsSystem = true
		};

		success = dboManager.Insert<Role>(role);
		if (!success)
			throw new DatabaseException("Failed to insert initial Administrator role record.");

		var userrole = new UserRole
		{
			UserId = user.Id,
			RoleId = role.Id
		};

		success = dboManager.Insert<UserRole>(userrole);
		if (!success)
			throw new DatabaseException("Failed to insert initial relation for administrator user and role record.");

	}
}
