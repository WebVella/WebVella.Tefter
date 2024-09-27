namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.7.22.1")]
internal class TefterSystemMigration2024072201 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: shared_column

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("shared_key_db_name", c => { c.NotNullable(); })
					.AddShortTextColumn("db_name", c => { c.NotNullable(); })
					.AddShortIntegerColumn("db_type", c => { c.NotNullable(); })
					.AddBooleanColumn("include_table_search", c => { c.NotNullable().WithDefaultValue(false); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_shared_column_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_shared_column_shared_key_db_name", i => { i.WithColumns("shared_key_db_name"); });
			});

		#endregion

		#region  TABLE: shared_column_short_text_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_short_text_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddShortTextColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					
					.AddPrimaryKeyConstraint("pk_shared_column_short_text_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })
					
					.AddForeignKeyConstraint("fk_shared_column_short_text_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_short_text_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_short_text_value", 
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_text_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_text_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddTextColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_text_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_text_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_text_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_text_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_boolean_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_boolean_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddBooleanColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_boolean_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_boolean_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_boolean_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_boolean_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_guid_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_guid_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddGuidColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_guid_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_guid_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_guid_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_guid_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_short_integer_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_short_integer_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddShortIntegerColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_short_integer_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_short_integer_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_short_integer_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_short_integer_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_integer_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_integer_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddIntegerColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_integer_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_integer_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_integer_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_integer_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_long_integer_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_long_integer_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddLongIntegerColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_long_integer_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_long_integer_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_long_integer_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_long_integer_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_number_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_number_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddNumberColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_number_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_number_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_number_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_number_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_date_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_date_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddDateColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_date_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_date_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_date_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_date_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: shared_column_datetime_value

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "shared_column_datetime_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("shared_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddDateTimeColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_datetime_value",
						c => { c.WithColumns("shared_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_datetime_value_1",
						fk => {
							fk
								.WithForeignTable("shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_datetime_value_2",
						fk => {
							fk
								.WithForeignTable("id_dict")
								.WithForeignColumns("id")
								.WithColumns("shared_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_datetime_value",
						i => { i.WithColumns("shared_key_id", "shared_column_id"); });
			});

		#endregion
	}
}
