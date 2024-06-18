namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.6.14.1")]
internal class TefterSystemMigration2024061801 : ITefterSystemMigration
{
	public void Migrate(IDatabaseManager dbManager, IDboManager dboManager)
	{
		var dbBuilder = dbManager.GetDatabaseBuilder();

		// MIGRATION
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "migration")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("addon_id", c => { c.WithoutAutoDefaultValue().Nullable(); })
					.AddTextColumn("migration_class_name", c => { c.NotNullable(); })
					.AddDateTimeColumn("executed_on", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("version", c => { c.NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_migration_id", c => { c.WithColumns("id"); });
			});

		// USER
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
					.AddUniqueKeyConstraint("ux_user_email", c => { c.WithColumns("email"); })
					.AddForeignKeyConstraint("fk_user_user_role", c =>
						{
							c
							.WithColumns("id")
							.WithForeignTable("user_role")
							.WithForeignColumns("id");

						});
			});

		// ROLE
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "role")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("name", c => { c.NotNullable(); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_role_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_role_name", c => { c.WithColumns("email"); })
					.AddForeignKeyConstraint("fk_role_user_role", c =>
					{
						c
						.WithColumns("id")
						.WithForeignTable("user_role")
						.WithForeignColumns("id");

					});
			});

		// USER_ROLE
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
					.AddPrimaryKeyConstraint("pk_role_id", c => { c.WithColumns("user_id", "role_id"); });
			});
	}
}
