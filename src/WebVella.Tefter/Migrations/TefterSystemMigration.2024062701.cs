namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.6.27.1")]
internal class TefterSystemMigration2024062701 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: MIGRATION
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

		#endregion

		#region TABLE: USER
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "user")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("email", c => { c.NotNullable(); })
					.AddShortTextColumn("password", c => { c.NotNullable(); })
					.AddShortTextColumn("first_name", c => { c.NotNullable().WithDefaultValue(""); })
					.AddShortTextColumn("last_name", c => { c.NotNullable().WithDefaultValue(""); })
					.AddTextColumn("settings_json", c => { c.NotNullable().WithDefaultValue("{}"); })
					.AddBooleanColumn("enabled", c => { c.NotNullable().WithDefaultValue(true); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddTextColumn("x_search", c => { c.NotNullable().WithDefaultValue(""); });
			})
			.WithIndexes(indexes =>
			{
				indexes.AddGinIndex("idx_user_x_search", i => { i.WithColumns("x_search"); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_user_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_user_email", c => { c.WithColumns("email"); });
			});

		#endregion

		#region TABLE: ROLE
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "role")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddBooleanColumn("is_system", c => { c.NotNullable().WithDefaultValue(false); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_role_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_role_name", c => { c.WithColumns("name"); });

			});
		#endregion

		#region TABLE: USER_ROLE
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
		#endregion

		#region TABLE: DATA_PROVIDER
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_provider")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("name", c => { c.NotNullable(); })
					.AddAutoIncrementColumn("index")
					.AddGuidColumn("type_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddTextColumn("type_name", c => { c.NotNullable(); })
					.AddTextColumn("settings_json", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_data_provider_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_data_provider_index", c => { c.WithColumns("index"); });
			});
		#endregion

		#region TABLE: DATA_PROVIDER_COLUMN
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_provider_column")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("data_provider_id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("source_name", c => { c.NotNullable(); })
					.AddTextColumn("source_type", c => { c.NotNullable(); })
					.AddTextColumn("db_name", c => { c.NotNullable(); })
					.AddShortIntegerColumn("db_type", c => { c.NotNullable(); })
					.AddTextColumn("default_value", c => { c.Nullable(); })
					.AddBooleanColumn("auto_default_value", c => { c.NotNullable().WithDefaultValue(false); })
					.AddBooleanColumn("is_nullable", c => { c.NotNullable().WithDefaultValue(false); })
					.AddBooleanColumn("is_sortable", c => { c.NotNullable().WithDefaultValue(false); })
					.AddBooleanColumn("is_unique", c => { c.WithDefaultValue(false).NotNullable(); })
					.AddBooleanColumn("is_searchable", c => { c.NotNullable().WithDefaultValue(false); })
					.AddShortIntegerColumn("preferred_search_type", c => { c.NotNullable().WithDefaultValue(0); })
					.AddBooleanColumn("include_in_table_search", c => { c.NotNullable().WithDefaultValue(false); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_data_provider_column_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_data_provider_column_db_name", c => { c.WithColumns("data_provider_id", "db_name"); })
					.AddForeignKeyConstraint("fk_data_provider_data_provider_column", c =>
					{
						c
						.WithColumns("data_provider_id")
						.WithForeignTable("data_provider")
						.WithForeignColumns("id");
					});
			}); 
		#endregion


	}

	public override async Task MigrateDataAsync(IServiceProvider serviceProvider)
	{
		IDatabaseService dbService = serviceProvider.GetService<IDatabaseService>();
		IDboManager dboManager = serviceProvider.GetService<IDboManager>();
		IIdentityManager identityManager = serviceProvider.GetService<IIdentityManager>();

		// CREATES INITIAL ADMINISTRATOR USER AND ROLE 

		{
			var adminRole = identityManager
				.CreateRoleBuilder()
				.WithName("Administrators")
				.IsSystem(true)
			.Build();

			var adminRoleResult = await identityManager.SaveRoleAsync(adminRole);
			if (!adminRoleResult.IsSuccess)
				throw new DatabaseException("Failed to create administrator role.");

			adminRole = adminRoleResult.Value;

			//default settings
			var user = identityManager
				.CreateUserBuilder()
				.WithEmail("admin@tefter.bg")
				.WithFirstName("Tefter")
				.WithLastName("Administrator")
				.CreatedOn(DateTime.Now)
				.WithPassword("123")
				.Enabled(true)
				.WithRoles(adminRole)
				.Build();

			var userResult = await identityManager.SaveUserAsync(user);
			if (!userResult.IsSuccess)
				throw new DatabaseException("Failed to create administrator user");
		}


		//adds users for developers
		//this migration will be removed for production or at later stage

#if DEBUG

		{

			var adminRoleResult = identityManager.GetRole("Administrators");
			if (!adminRoleResult.IsSuccess)
				throw new DatabaseException("Failed to get admin role.");

			var adminRole = adminRoleResult.Value;

			var user = identityManager
				.CreateUserBuilder()
				.WithEmail("rumen@webvella.com")
				.WithFirstName("Rumen")
				.WithLastName("Yankov")
				.CreatedOn(DateTime.Now)
				.WithPassword("123")
				.Enabled(true)
				.WithRoles(adminRole)
				.Build();

			var userResult = await identityManager.SaveUserAsync(user);
			if (!userResult.IsSuccess)
				throw new DatabaseException("Failed to create Rumen Yankov user");

			user = identityManager
			   .CreateUserBuilder()
			   .WithEmail("boz@webvella.com")
			   .WithFirstName("Bozhidar")
			   .WithLastName("Zashev")
			   .CreatedOn(DateTime.Now)
			   .WithPassword("123")
			   .Enabled(true)
			   .WithRoles(adminRole)
			   .Build();

			userResult = await identityManager.SaveUserAsync(user);
			if (!userResult.IsSuccess)
				throw new DatabaseException("Failed to create Bozhidar Zashev user");
		}

#endif
	}
}
