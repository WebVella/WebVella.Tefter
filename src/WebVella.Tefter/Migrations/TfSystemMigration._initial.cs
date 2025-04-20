namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2025.04.09.1")]
internal class TefterSystemMigration2025040901 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
	{
		#region  TABLE: MIGRATION
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_migration")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("application_id", c => { c.WithoutAutoDefaultValue().Nullable(); })
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
			.NewTableBuilder(Guid.NewGuid(), "tf_user")
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
			.NewTableBuilder(Guid.NewGuid(), "tf_role")
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
			.NewTableBuilder(Guid.NewGuid(), "tf_user_role")
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
						.WithForeignTable("tf_role")
						.WithForeignColumns("id");
					})
					.AddForeignKeyConstraint("fk_user_role_user", c =>
					{
						c
						.WithColumns("user_id")
						.WithForeignTable("tf_user")
						.WithForeignColumns("id");
					});
			});
		#endregion

		#region TABLE: DATA_PROVIDER
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_data_provider")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("name", c => { c.NotNullable(); })
					.AddAutoIncrementColumn("index")
					.AddGuidColumn("type_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddTextColumn("settings_json", c => { c.Nullable(); })
					.AddTextColumn("sync_primary_key_columns_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddShortIntegerColumn("synch_schedule_minutes", c => { c.NotNullable().WithDefaultValue(60); })
					.AddBooleanColumn("synch_schedule_enabled", c => { c.NotNullable().WithDefaultValue(false); });
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
			.NewTableBuilder(Guid.NewGuid(), "tf_data_provider_column")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("data_provider_id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("source_name", c => { c.Nullable(); })
					.AddTextColumn("source_type", c => { c.Nullable(); })
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
						.WithForeignTable("tf_data_provider")
						.WithForeignColumns("id");
					});
			});
		#endregion

		#region  TABLE: DATA_PROVIDER_JOIN_KEY
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_data_provider_join_key")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("data_provider_id", c => { c.NotNullable().WithoutAutoDefaultValue(); })
					.AddShortTextColumn("db_name", c => { c.NotNullable(); })
					.AddShortTextColumn("description", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("column_ids_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddShortIntegerColumn("version", c => { c.NotNullable().WithDefaultValue(1); })
					.AddDateTimeColumn("last_modified_on", c => { c.NotNullable().WithAutoDefaultValue(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_data_provider_join_key", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_data_provider_join_key_data_provider", c =>
					{
						c.WithColumns("data_provider_id")
						.WithForeignTable("tf_data_provider")
						.WithForeignColumns("id");
					});
			});

		#endregion

		#region  TABLE: ID_DICT

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_id_dict")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("text_id", c => { c.NotNullable().WithDefaultValue(""); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_id_dict", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_id_dict_text_id_ux", c => { c.WithColumns("text_id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_id_dict_id", i => { i.WithColumns("id"); })
					.AddHashIndex("ux_id_dict_text_id", c => { c.WithColumn("text_id"); });
			});

		#endregion

		#region  TABLE: DATA_PROVIDER_SYNCHRONIZE_TASK

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_data_provider_synchronize_task")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("data_provider_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddTextColumn("policy_json", c => { c.NotNullable().WithDefaultValue("{}"); })
					.AddShortIntegerColumn("status", c => { c.NotNullable(); })
					.AddTextColumn("synch_log_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddDateTimeColumn("started_on", c => { c.Nullable().WithoutAutoDefaultValue(); })
					.AddDateTimeColumn("completed_on", c => { c.Nullable().WithoutAutoDefaultValue(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_dp_synchronize_task_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_dp_task_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_dp_synchronize_task_provider_id", i => { i.WithColumns("data_provider_id"); })
					.AddBTreeIndex("ix_dp_synchronize_task_created_on", i => { i.WithColumns("created_on"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("join_key_db_name", c => { c.NotNullable(); })
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
					.AddBTreeIndex("ix_shared_column_join_key_db_name", i => { i.WithColumns("join_key_db_name"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_SHORT_TEXT_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_short_text_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddShortTextColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_short_text_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_short_text_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_short_text_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_short_text_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_TEXT_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_text_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddTextColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_text_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_text_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_text_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_text_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_BOOLEAN_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_boolean_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddBooleanColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_boolean_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_boolean_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_boolean_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_boolean_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_GUID_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_guid_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddGuidColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_guid_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_guid_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_guid_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_guid_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_SHORT_INTEGER_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_short_integer_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddShortIntegerColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_short_integer_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_short_integer_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_short_integer_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_short_integer_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_INTEGER_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_integer_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddIntegerColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_integer_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_integer_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_integer_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_integer_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_LONG_INTEGER_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_long_integer_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddLongIntegerColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_long_integer_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_long_integer_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_long_integer_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_long_integer_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_NUMBER_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_number_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddNumberColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_number_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_number_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_number_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_number_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_DATE_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_date_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddDateColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_date_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_date_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_date_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_date_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SHARED_COLUMN_DATETIME_VALUE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_shared_column_datetime_value")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("join_key_id", c => { c.NotNullable(); })
					.AddGuidColumn("shared_column_id", c => { c.NotNullable(); })
					.AddDateTimeColumn("value", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints

					.AddPrimaryKeyConstraint("pk_shared_column_datetime_value",
						c => { c.WithColumns("join_key_id", "shared_column_id"); })

					.AddForeignKeyConstraint("fk_shared_column_datetime_value_1",
						fk =>
						{
							fk
								.WithForeignTable("tf_shared_column")
								.WithForeignColumns("id")
								.WithColumns("shared_column_id");
						})

					.AddForeignKeyConstraint("fk_shared_column_datetime_value_2",
						fk =>
						{
							fk
								.WithForeignTable("tf_id_dict")
								.WithForeignColumns("id")
								.WithColumns("join_key_id");
						});

			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_shared_column_datetime_value",
						i => { i.WithColumns("join_key_id", "shared_column_id"); });
			});

		#endregion

		#region  TABLE: SPACE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_space")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
					.AddBooleanColumn("is_private", c => { c.NotNullable().WithDefaultValue(false); })
					.AddShortTextColumn("icon", c => { c.Nullable(); })
					.AddShortIntegerColumn("color", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_space_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_space_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_space_name", i => { i.WithColumns("name"); });
			});

		#endregion

		#region  TABLE: SPACE_DATA

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_space_data")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("space_id", c => { c.NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
					.AddGuidColumn("data_provider_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddTextColumn("filters_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddTextColumn("columns_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddTextColumn("sort_orders_json", c => { c.NotNullable().WithDefaultValue("[]"); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_space_data_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_space_data_space", c =>
					{
						c
						.WithForeignTable("tf_space")
						.WithForeignColumns("id")
						.WithColumns("space_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_space_data_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_space_data_space_id", i => { i.WithColumns("space_id"); })
					.AddBTreeIndex("ix_space_data_name", i => { i.WithColumns("name"); });
			});

		#endregion

		#region  TABLE: SPACE_VIEW

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_space_view")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
					.AddGuidColumn("space_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("space_data_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddTextColumn("settings_json", c => { c.NotNullable().WithDefaultValue("{}"); })
					.AddTextColumn("presets_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddTextColumn("groups_json", c => { c.NotNullable().WithDefaultValue("[]"); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_space_view_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_space_view_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_space_view_name", i => { i.WithColumns("name"); });
			});

		#endregion

		#region  TABLE: SPACE_VIEW_COLUMN

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_space_view_column")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("space_view_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("query_name", c => { c.NotNullable(); })
					.AddShortTextColumn("title", c => { c.NotNullable(); })
					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
					.AddGuidColumn("type_id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("component_id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("data_mapping_json", c => { c.NotNullable(); })
					.AddTextColumn("custom_options_json", c => { c.NotNullable(); })
					.AddTextColumn("settings_json", c => { c.NotNullable().WithDefaultValue("{}"); })
					.AddShortTextColumn("icon", c => { c.NotNullable().WithDefaultValue(""); })
					.AddBooleanColumn("only_icon", c => { c.NotNullable().WithDefaultValue(false); });


			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_space_view_column_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_space_view_column_space_view", c =>
					{
						c
						.WithForeignTable("tf_space_view")
						.WithForeignColumns("id")
						.WithColumns("space_view_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_space_view_column_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_space_view_column_space_view_id", i => { i.WithColumns("space_view_id"); });
			});

		#endregion

		#region  TABLE: BOOKMARK

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_bookmark")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddTextColumn("description", c => { c.NotNullable(); })
					.AddTextColumn("url", c => { c.Nullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("user_id", c => { c.NotNullable(); })
					.AddGuidColumn("space_view_id", c => { c.NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_bookmark_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_bookmark_space_view", c =>
					{
						c.WithForeignTable("tf_space_view")
						.WithForeignColumns("id")
						.WithColumns("space_view_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_bookmark_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_bookmark_user_id", i => { i.WithColumns("user_id"); });
			});

		#endregion

		#region  TABLE: TAG

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_tag")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("label", c => { c.NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_tag_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_tag_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_tag_label", i => { i.WithColumns("label"); });
			});

		#endregion

		#region  TABLE: BOOKMARK_TAG

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_bookmark_tag")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("bookmark_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("tag_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_bookmark_tags", c => { c.WithColumns("bookmark_id", "tag_id"); })
					.AddForeignKeyConstraint("fk_bookmark_tags_bookmark", c =>
					{
						c.WithForeignTable("tf_bookmark")
						.WithForeignColumns("id")
						.WithColumns("bookmark_id");
					})
					.AddForeignKeyConstraint("fk_bookmark_tags_tag", c =>
					{
						c.WithForeignTable("tf_tag")
						.WithForeignColumns("id")
						.WithColumns("tag_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_bookmark_tags_bookmark_id", i => { i.WithColumns("bookmark_id"); })
					.AddBTreeIndex("ix_bookmark_tags_tag_id", i => { i.WithColumns("tag_id"); });
			});

		#endregion

		#region  TABLE: SPACE_PAGE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_space_page")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("parent_id", c => { c.Nullable().WithoutAutoDefaultValue(); })
					.AddGuidColumn("space_id", c => { c.NotNullable().WithoutAutoDefaultValue(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortTextColumn("icon", c => { c.NotNullable(); })
					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
					.AddGuidColumn("component_id", c => { c.Nullable(); })
					.AddTextColumn("component_settings_json", c => { c.NotNullable().WithDefaultValue("{}"); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_space_page_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_space_page_space", c =>
					{
						c.WithForeignTable("tf_space")
						.WithForeignColumns("id")
						.WithColumns("space_id");
					})
					.AddForeignKeyConstraint("fk_space_page_parent", c =>
					{
						c.WithForeignTable("tf_space_page")
						.WithForeignColumns("id")
						.WithColumns("parent_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_space_page_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_space_page_space_id", i => { i.WithColumns("space_id"); });
			});

		#endregion

		#region  TABLE: REPOSITORY_FILE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_repository_file")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("filename", c => { c.NotNullable(); })
					.AddGuidColumn("created_by", c => { c.Nullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("last_modified_by", c => { c.Nullable(); })
					.AddDateTimeColumn("last_modified_on", c => { c.NotNullable().WithAutoDefaultValue(); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_repository_files_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_repository_file_id", i => { i.WithColumns("id"); })
					.AddHashIndex("ix_repository_file_filename", i => { i.WithColumn("filename"); });
			});

		#endregion

		#region  TABLE: TEMPLATE

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tf_template")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortTextColumn("icon", c => { c.Nullable(); })
					.AddTextColumn("description", c => { c.Nullable().WithDefaultValue(""); })
					.AddTextColumn("space_data_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddBooleanColumn("is_enabled", c => { c.NotNullable().WithDefaultValue(true); })
					.AddBooleanColumn("is_selectable", c => { c.NotNullable().WithDefaultValue(true); })
					.AddShortIntegerColumn("result_type", c => { c.NotNullable().WithDefaultValue(0); })
					.AddTextColumn("settings_json", c => { c.Nullable(); })
					.AddTextColumn("content_processor_type", c => { c.NotNullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddDateTimeColumn("modified_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("created_by", c => { c.Nullable(); })
					.AddGuidColumn("modified_by", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_template_id", c => { c.WithColumns("id"); });
			});

		#endregion
	}

	public override async Task MigrateDataAsync(IServiceProvider serviceProvider)
	{
		ITfDatabaseService dbService = serviceProvider.GetService<ITfDatabaseService>();
		ITfDboManager dboManager = serviceProvider.GetService<ITfDboManager>();
		ITfService tfService = serviceProvider.GetService<ITfService>();

		//creates default empty id in id_dict
		//this entry is required because data provider join keys default
		//values are Guid.Empty and they have foreign key to id_dict

		dbService.ExecuteSqlNonQueryCommand("INSERT INTO tf_id_dict(id,text_id) VALUES(@id,@text_id)",
			new NpgsqlParameter("id", Guid.Empty), new NpgsqlParameter("text_id", Guid.Empty.ToString()));

		// CREATES INITIAL ADMINISTRATOR USER AND ROLE 

		{
			var adminRole = tfService
				.CreateRoleBuilder()
				.WithName("Administrators")
				.IsSystem(true)
			.Build();

			adminRole = await tfService.SaveRoleAsync(adminRole);

			//default settings
			var user = tfService
				.CreateUserBuilder()
				.WithEmail("admin@tefter.bg")
				.WithFirstName("Tefter")
				.WithLastName("Administrator")
				.CreatedOn(DateTime.Now)
				.WithPassword("@tefter.bg")
				.Enabled(true)
				.WithRoles(adminRole)
				.WithThemeColor(OfficeColor.Excel)
				.WithThemeMode(DesignThemeModes.System)
				.WithOpenSidebar(true)
				.Build();

			await tfService.SaveUserAsync(user);
		}

	}
}