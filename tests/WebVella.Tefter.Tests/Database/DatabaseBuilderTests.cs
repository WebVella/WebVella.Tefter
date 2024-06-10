﻿namespace WebVella.Tefter.Tests.Database;

public partial class DatabaseBuilderTests : BaseTest
{
	#region <--- Table Tests --->

	[Fact]
	public void Table_CRUD()
	{
		Guid tableId = Guid.NewGuid();
		const string tableName = "test_table";
		Guid tableAppId = Guid.NewGuid();
		Guid tableDpId = Guid.NewGuid();

		var databaseBuilder = DatabaseBuilder.New();

		var tables = databaseBuilder
			.NewTable(tableId, tableName, table => { })
			.Build();

		tables.Count().Should().Be(1);

		var table = tables.Find(tableName);
		var tableById = tables.Find(tableId);

		table.Should().NotBeNull();
		tableById.Should().NotBeNull();

		tableById.Name.Should().Be(table.Name);
		table.Name.Should().Be(tableById.Name);

		table.Columns.Count().Should().Be(0);
		table.Indexes.Count().Should().Be(0);
		table.Constraints.Count().Should().Be(0);

		table.ApplicationId.Should().BeNull();
		table.DataProviderId.Should().BeNull();

		table = databaseBuilder
				.WithTableBuilder(tableId, t =>
				{
					t
					.WithApplicationId(tableAppId)
					.WithDataProviderId(tableDpId);
				})
				.Build();

		table.ApplicationId.Should().Be(tableAppId);
		table.DataProviderId.Should().Be(tableDpId);

		tables = databaseBuilder
			.Remove(tableName)
			.Build();

		tables.Count().Should().Be(0);
	}

	[Fact]
	public void Table_WithSameIds()
	{
		Guid tableId = Guid.NewGuid();
		const string tableName = "test_table";
		Guid tableAppId = Guid.NewGuid();
		Guid tableDpId = Guid.NewGuid();

		var databaseBuilder = DatabaseBuilder.New();

		var task = Task.Run(() =>
		{
			var tables = databaseBuilder
				.NewTable(tableId, tableName, table => { })
				.NewTable(tableId, tableName + "1", table => { })
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType(typeof(DbBuilderException));
		exception.Message.Should().Be($"There is already existing database object with id '{tableId}'");
	}

	[Fact]
	public void Table_WithSameNames()
	{
		Guid tableId1 = Guid.NewGuid();
		Guid tableId2 = Guid.NewGuid();
		const string tableName = "test_table";
		Guid tableAppId = Guid.NewGuid();
		Guid tableDpId = Guid.NewGuid();

		var databaseBuilder = DatabaseBuilder.New();

		var task = Task.Run(() =>
		{
			var tables = databaseBuilder
				.NewTable(tableId1, tableName, table => { })
				.NewTable(tableId2, tableName, table => { })
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType(typeof(DbBuilderException));
		exception.Message.Should().Be($"There is already existing database object with name '{tableName}'");
	}

	[Fact]
	public void Table_RemoveNonExisting()
	{
		const string tableName = "test_table";

		var databaseBuilder = DatabaseBuilder.New();

		var task = Task.Run(() =>
		{
			var tables = databaseBuilder
				.Remove(tableName)
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType(typeof(DbBuilderException));
		exception.Message.Should().Be($"Table with name '{tableName}' is not found.");
	}

	[Fact]
	public void Table_CreateSimpleStructure()
	{
		var tables = DatabaseBuilder.New()
			.NewTable("data1", table =>
			{
				table
					.WithColumns(columns =>
					{
						columns

						#region <--- AutoInc --->
							.AddAutoIncrementColumn("auto_inc1")
							.AddAutoIncrementColumn("auto_inc2")
						#endregion

						#region <--- Guid --->
							.AddGuidColumn("id", x => x.NotNullable().WithAutoDefaultValue())

							.AddGuidColumn("guid_nullable_with_auto_default", c =>
							{
								c.WithAutoDefaultValue()
								.Nullable();
							})
							.AddGuidColumn("guid_not_nullable_with_auto_default", c =>
							{
								c.WithAutoDefaultValue()
								.NotNullable();
							})
							.AddGuidColumn("guid_nullable_with_default_value", c =>
							{
								c.WithDefaultValue(Guid.NewGuid())
								.Nullable();
							})
							.AddGuidColumn("guid_not_nullable_with_default_value", c =>
							{
								c.WithDefaultValue(Guid.NewGuid())
								.NotNullable();
							})
							.AddGuidColumn("guid_nullable_with_default_value_and_auto", c =>
							{
								c.Nullable()
								.WithDefaultValue(Guid.NewGuid())
								.WithoutAutoDefaultValue();
							})
							.AddGuidColumn("guid_not_nullable_with_default_value_and_auto", c =>
							{
								c.WithDefaultValue(Guid.NewGuid())
								.WithoutAutoDefaultValue();
							})
							.AddGuidColumn("guid_nullable_without_default_value", c => { c.Nullable(); })
							//this should provide exception
							//.AddNewGuidColumn("guid_not_nullable_without_default_value", c => { c.Nullable(); }) 
						#endregion

						#region <--- Boolean --->
							.AddBooleanColumn("bool_not_nullable_with_default_true", c =>
							{
								c.WithDefaultValue(true);
							})
							.AddBooleanColumn("bool_not_nullable_with_default_false", c =>
							{
								c.WithDefaultValue(false);
							})
							.AddBooleanColumn("bool_nullable_default_true", c =>
							{
								c.WithDefaultValue(true).Nullable();
							})
							.AddBooleanColumn("bool_nullable_default_false", c =>
							{
								c.WithDefaultValue(true).Nullable();
							})
							.AddBooleanColumn("bool_nullable_without_default", c =>
							{
								c.Nullable();
							})
							//this should provide exception
							//.AddNewBooleanColumn("bool_not_nullable_without_default", c => { c.NotNullable(); })  
						#endregion

						#region <--- Number --->
							.AddNumberColumn("number_nullable_with_default", c =>
							{
								c.WithDefaultValue(2)
								.Nullable();
							})
							.AddNumberColumn("number_not_nullable_with_default", c =>
							{
								c.WithDefaultValue(1).NotNullable();
							})
							.AddNumberColumn("number_nullable_without_default", c =>
							{
								c.Nullable();
							})
							//this should provide exception
							//.AddNewNumberColumn("number_not_nullable_without_default", c => { c.NotNullable(); }) 
						#endregion

						#region <--- Date --->
							.AddDateColumn("date_nullable_with_auto_default", c =>
							{
								c.WithAutoDefaultValue()
								.Nullable();
							})
							.AddDateColumn("date_not_nullable_with_auto_default", c =>
							{
								c.WithAutoDefaultValue()
								.NotNullable();
							})
							.AddDateColumn("date_nullable_with_default_value", c =>
							{
								c.WithDefaultValue(new DateOnly(2000, 1, 1))
								.Nullable();
							})
							.AddDateColumn("date_not_nullable_with_default_value", c =>
							{
								c.WithDefaultValue(new DateOnly(2000, 1, 1))
								.NotNullable();
							})
							.AddDateColumn("date_nullable_with_default_value_and_auto", c =>
							{
								c.Nullable()
								.WithDefaultValue(new DateOnly(2000, 1, 1))
								.WithoutAutoDefaultValue();
							})
							.AddDateColumn("date_not_nullable_with_default_value_and_auto", c =>
							{
								c.WithDefaultValue(new DateOnly(2000, 1, 1))
								.WithoutAutoDefaultValue();
							})
							.AddDateColumn("date_nullable_without_default_value", c =>
							{
								c.Nullable();
							})
							//this should provide exception
							//.AddNewDateColumn("date_not_nullable_without_default_value", c => { c.NotNullable(); }) 
						#endregion

						#region <--- DateTime --->
							.AddDateTimeColumn("datetime_nullable_with_auto_default", c =>
							{
								c.WithAutoDefaultValue()
								.Nullable();
							})
							.AddDateTimeColumn("datetime_not_nullable_with_auto_default", c =>
							{
								c.WithAutoDefaultValue()
								.NotNullable();
							})
							.AddDateTimeColumn("datetime_nullable_with_default_value", c =>
							{
								c.WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
								.Nullable();
							})
							.AddDateTimeColumn("datetime_not_nullable_with_default_value", c =>
							{
								c.WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
								.NotNullable();
							})
							.AddDateTimeColumn("datetime_nullable_with_default_value_and_auto", c =>
							{
								c.Nullable()
								.WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
								.WithoutAutoDefaultValue();
							})
							.AddDateTimeColumn("datetime_not_nullable_with_default_value_and_auto", c =>
							{
								c.WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
								.WithoutAutoDefaultValue();
							})
							.AddDateTimeColumn("datetime_nullable_without_default_value", c =>
							{
								c.Nullable();
							})
							//this should provide exception
							//.AddNewDateTimeColumn("datetime_not_nullable_without_default_value", c => { c.NotNullable(); }) 
						#endregion

						#region <--- Text --->
							.AddTextColumn("test_nullable_with_default", c =>
							{
								c.WithDefaultValue("default1")
								.Nullable();
							})
							.AddTextColumn("text_not_nullable_with_default", c =>
							{
								c.WithDefaultValue("default2")
								.NotNullable();
							})
							.AddTextColumn("text_nullable_without_default", c =>
							{
								c.Nullable();
							});
						//this should provide exception
						//.AddNewTextColumn("text_not_nullable_without_default", c => { c.NotNullable(); }) 
						#endregion

					})
					.WithIndexes(indexes =>
					{
						indexes
							.AddBTreeIndex("idx_btree_id", i => i.WithColumns("id"))
							.AddHashIndex("idx_hash", i => i.WithColumn("text_not_nullable_with_default"))
							.AddGinIndex("idx_gin", i => i.WithColumns("text_not_nullable_with_default"))
							.AddGistIndex("idx_gist", i => i.WithColumns("text_not_nullable_with_default"));
					})
					.WithConstraints(constraints =>
					{
						constraints
							.AddPrimaryKeyConstraint("pk_data1_id", c => c.WithColumns("id"))
							.AddUniqueKeyConstraint("ux_text_not_nullable_with_default", c => c.WithColumns("text_not_nullable_with_default"));
					});
			})
			 .NewTable("data2", table =>
			 {
				 table
					 .WithColumns(columns =>
					 {
						 columns
						 .AddGuidColumn("id", x => x.NotNullable().WithAutoDefaultValue())
						 .AddTextColumn("text", column =>
						 {
							 column
								.WithDefaultValue("test")
								.NotNullable();
						 });

					 })
					 .WithIndexes(indexes =>
					 {
						 indexes
							.AddBTreeIndex("idx_data2_id", i => i.WithColumns("id"));
					 })
					 .WithConstraints(constraints =>
					 {
						 constraints
							 .AddPrimaryKeyConstraint("pk_data2_id", c => c.WithColumns("id"));
					 });
			 })
			 .NewTable("rel_table", table =>
			 {
				 table
					 .WithColumns(columns =>
					 {
						 columns
							.AddGuidColumn("origin_id", column => { column.NotNullable(); })
							.AddGuidColumn("target_id", column => { column.NotNullable(); });
					 })
					 .WithConstraints(constraints =>
					 {
						 constraints
							 .AddPrimaryKeyConstraint("pk_rel_table", c => c.WithColumns("origin_id", "target_id"))
							 .AddUniqueKeyConstraint("ux_rel_table", c => c.WithColumns("origin_id", "target_id"))
							 .AddForeignKeyConstraint("fk_rel_table_data1", c =>
							 {
								 c.WithColumns("origin_id")
								 .WithForeignTable("data1")
								 .WithForeignColumns("id");
							 })
							  .AddForeignKeyConstraint("fk_rel_table_data2", c =>
							  {
								  c.WithColumns("target_id")
								  .WithForeignTable("data2")
								  .WithForeignColumns("id");
							  });
					 });
			 })
			.Build();

		var differences = DatabaseComparer.Compare(null, tables);
		var createSql = DbSqlProvider.GenerateUpdateScript(differences);

		var tables2 = DatabaseBuilder.New(tables)
			.WithTable("data1", table =>
			{
				table.WithColumns(columns =>
				{
					columns.Remove("id");
				});
			}).Build();


		differences = DatabaseComparer.Compare(tables, tables2);
		var dropSql = DbSqlProvider.GenerateUpdateScript(differences);

		var tables3 = DatabaseBuilder.New(tables2)
		   .WithTable("data2", table =>
		   {
			   table.WithColumns(columns =>
			   {
				   columns.Remove("id");
			   });
		   }).Build();

		differences = DatabaseComparer.Compare(tables2, tables3);
		var dropModeSql = DbSqlProvider.GenerateUpdateScript(differences);

	}

	#endregion

	#region <--- Column Tests --->

	[Fact]
	public void Column_CRUD_Guid()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "test_guid";
		Guid defaultValue = Guid.NewGuid();

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.AddGuidColumn(columnId, columnName, column =>
					{
						column
							.WithoutAutoDefaultValue()
							.WithDefaultValue(defaultValue)
							.Nullable();
					});
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		var columnById = table.Columns.Find(columnId);
		var columnByName = table.Columns.Find(columnName);

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());
		columnById.Should().BeOfType<DbGuidColumn>();

		columnById.IsNullable.Should().BeTrue();
		columnById.DefaultValue.Should().Be(defaultValue);
		((DbGuidColumn)columnById).AutoDefaultValue.Should().Be(false);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.WithGuidColumn(columnName, column =>
					{
						column
							.WithAutoDefaultValue()
							.WithDefaultValue(null)
							.NotNullable();
					});
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		columnById = table.Columns.Find(columnId);
		columnByName = table.Columns.Find(columnName);
		columnById.Should().BeOfType<DbGuidColumn>();

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());

		columnById.IsNullable.Should().BeFalse();
		columnById.DefaultValue.Should().Be(null);
		((DbGuidColumn)columnById).AutoDefaultValue.Should().BeTrue();

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.Remove(columnName);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(0);
	}

	[Fact]
	public void Column_CRUD_Number()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "test_number";
		decimal defaultValue = 10;

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.AddNumberColumn(columnId, columnName, column =>
					{
						column
							.WithDefaultValue(defaultValue)
							.Nullable();
					});
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		var columnById = table.Columns.Find(columnId);
		var columnByName = table.Columns.Find(columnName);

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());
		columnById.Should().BeOfType<DbNumberColumn>();

		columnById.IsNullable.Should().BeTrue();
		columnById.DefaultValue.Should().Be(defaultValue);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.WithNumberColumn(columnName, column =>
					{
						column
							.WithDefaultValue(null)
							.NotNullable();
					});
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		columnById = table.Columns.Find(columnId);
		columnByName = table.Columns.Find(columnName);
		columnById.Should().BeOfType<DbNumberColumn>();

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());

		columnById.IsNullable.Should().BeFalse();
		columnById.DefaultValue.Should().Be(null);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.Remove(columnName);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(0);
	}

	[Fact]
	public void Column_CRUD_Boolean()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "test_boolean";
		bool defaultValue = true;

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.AddBooleanColumn(columnId, columnName, column =>
					{
						column
							.WithDefaultValue(defaultValue)
							.Nullable();
					});
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		var columnById = table.Columns.Find(columnId);
		var columnByName = table.Columns.Find(columnName);

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());
		columnById.Should().BeOfType<DbBooleanColumn>();

		columnById.IsNullable.Should().BeTrue();
		columnById.DefaultValue.Should().Be(defaultValue);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.WithBooleanColumn(columnName, column =>
					{
						column
							.WithDefaultValue(null)
							.NotNullable();
					});
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		columnById = table.Columns.Find(columnId);
		columnByName = table.Columns.Find(columnName);
		columnById.Should().BeOfType<DbBooleanColumn>();

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());

		columnById.IsNullable.Should().BeFalse();
		columnById.DefaultValue.Should().Be(null);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.Remove(columnName);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(0);
	}

	[Fact]
	public void Column_CRUD_Text()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "test_text";
		string defaultValue = "def";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.AddTextColumn(columnId, columnName, column =>
					{
						column
							.WithDefaultValue(defaultValue)
							.Nullable();
					});
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		var columnById = table.Columns.Find(columnId);
		var columnByName = table.Columns.Find(columnName);

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());
		columnById.Should().BeOfType<DbTextColumn>();

		columnById.IsNullable.Should().BeTrue();
		columnById.DefaultValue.Should().Be(defaultValue);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.WithTextColumn(columnName, column =>
					{
						column
							.WithDefaultValue(null)
							.NotNullable();
					});
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		columnById = table.Columns.Find(columnId);
		columnByName = table.Columns.Find(columnName);
		columnById.Should().BeOfType<DbTextColumn>();

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());

		columnById.IsNullable.Should().BeFalse();
		columnById.DefaultValue.Should().Be(null);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.Remove(columnName);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(0);
	}

	[Fact]
	public void Column_CRUD_Date()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "test_text";
		DateOnly defaultValue = new DateOnly(2024, 10, 25);

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.AddDateColumn(columnId, columnName, column =>
					{
						column
							.WithDefaultValue(defaultValue)
							.Nullable();
					});
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		var columnById = table.Columns.Find(columnId);
		var columnByName = table.Columns.Find(columnName);

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());
		columnById.Should().BeOfType<DbDateColumn>();

		columnById.IsNullable.Should().BeTrue();
		columnById.DefaultValue.Should().Be(defaultValue);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.WithDateColumn(columnName, column =>
					{
						column
							.WithDefaultValue(null)
							.NotNullable();
					});
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		columnById = table.Columns.Find(columnId);
		columnByName = table.Columns.Find(columnName);
		columnById.Should().BeOfType<DbDateColumn>();

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());

		columnById.IsNullable.Should().BeFalse();
		columnById.DefaultValue.Should().Be(null);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.Remove(columnName);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(0);
	}

	[Fact]
	public void Column_CRUD_DateTime()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "test_text";
		DateTime defaultValue = DateTime.Now;

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.AddDateTimeColumn(columnId, columnName, column =>
					{
						column
							.WithDefaultValue(defaultValue)
							.Nullable();
					});
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		var columnById = table.Columns.Find(columnId);
		var columnByName = table.Columns.Find(columnName);

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());
		columnById.Should().BeOfType<DbDateTimeColumn>();

		columnById.IsNullable.Should().BeTrue();
		columnById.DefaultValue.Should().Be(defaultValue);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.WithDateTimeColumn(columnName, column =>
					{
						column
							.WithDefaultValue(null)
							.NotNullable();
					});
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(1);
		columnById = table.Columns.Find(columnId);
		columnByName = table.Columns.Find(columnName);
		columnById.Should().BeOfType<DbDateTimeColumn>();

		columnById.Should().NotBeNull();
		columnByName.Should().NotBeNull();
		columnById.GetHashCode().Should().Be(columnById.GetHashCode());

		columnById.IsNullable.Should().BeFalse();
		columnById.DefaultValue.Should().Be(null);

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns
					.Remove(columnName);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Should().NotBeNull();
		table.Columns.Should().HaveCount(0);
	}

	[Fact]
	public void Column_WithSameId()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.AddDateColumn(columnId, "c1", null);
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.AddDateColumn(columnId, "c2", null);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"There is already existing database object with id '{columnId}'");
	}

	[Fact]
	public void Column_WithSameName()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		const string columnName = "test";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.AddDateColumn(columnName, null);
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.AddDateColumn(columnName, null);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"There is already existing column with name '{columnName}' for table '{tableBuilder.Name}'");
	}

	[Fact]
	public void Column_WithDiffType()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		const string columnName = "test";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.AddDateColumn(columnName, null);
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.WithBooleanColumn(columnName, null);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"Column of type Boolean and name '{columnName}' for table '{table.Name}' is not found.");
	}

	[Fact]
	public void Column_RemoveNonExisting()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		const string columnName = "test";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.AddDateColumn(columnName, null);
			})
			.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		string nonExistingColumnName = columnName + "1";
		task = Task.Run(() =>
		{
			table = tableBuilder.WithColumns(columns =>
			{
				columns.Remove(nonExistingColumnName);
			})
			.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"Column with name '{nonExistingColumnName}' for table '{table.Name}' is not found.");
	}
	#endregion

	#region <--- Index Tests --->

	[Fact]
	public void Index_BTree_CRUD()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "id";
		Guid defaultValue = Guid.NewGuid();
		const string indexName = "idx_btree";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddGuidColumn(columnId, columnName, column =>
						{
							column
								.WithAutoDefaultValue()
								.NotNullable();
						});
				})
				.WithIndexes(indexes =>
				{
					indexes
					.AddBTreeIndex(indexName, index =>
						{
							index.WithColumns(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(1);
		table.Indexes.Find(indexName).Should().NotBeNull();
		table.Indexes.Find(indexName).Should().BeOfType<DbBTreeIndex>();
		table.Indexes.Find(indexName).Columns.Should().Contain(columnName);

		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithIndexes(indexes =>
				{
					indexes.Remove(indexName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(0);
	}

	[Fact]
	public void Index_Gin_CRUD()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "text";
		const string defaultValue = "test default value";
		const string indexName = "idx_gin";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddTextColumn(columnId, columnName, column =>
						{
							column
								.WithDefaultValue(defaultValue)
								.NotNullable();
						});
				})
				.WithIndexes(indexes =>
				{
					indexes
						.AddGinIndex(indexName, index =>
						{
							index.WithColumns(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(1);
		table.Indexes.Find(indexName).Should().NotBeNull();
		table.Indexes.Find(indexName).Should().BeOfType<DbGinIndex>();
		table.Indexes.Find(indexName).Columns.Should().Contain(columnName);


		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithIndexes(indexes =>
				{
					indexes.Remove(indexName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(0);
	}

	[Fact]
	public void Index_Gist_CRUD()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "text";
		const string defaultValue = "test default value";
		const string indexName = "idx_gist";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddTextColumn(columnId, columnName, column =>
						{
							column
								.WithDefaultValue(defaultValue)
								.NotNullable();
						});
				})
				.WithIndexes(indexes =>
				{
					indexes
						.AddGistIndex(indexName, index =>
						{
							index.WithColumns(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(1);
		table.Indexes.Find(indexName).Should().NotBeNull();
		table.Indexes.Find(indexName).Should().BeOfType<DbGistIndex>();
		table.Indexes.Find(indexName).Columns.Should().Contain(columnName);



		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithIndexes(indexes =>
				{
					indexes.Remove(indexName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(0);
	}

	[Fact]
	public void Index_Hash_CRUD()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "text";
		const string defaultValue = "test default value";
		const string indexName = "idx_hash";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddTextColumn(columnId, columnName, column =>
						{
							column
								.WithDefaultValue(defaultValue)
								.NotNullable();
						});
				})
				.WithIndexes(indexes =>
				{
					indexes
						.AddHashIndex(indexName, index =>
						{
							index.WithColumn(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(1);
		table.Indexes.Find(indexName).Should().NotBeNull();
		table.Indexes.Find(indexName).Should().BeOfType<DbHashIndex>();
		table.Indexes.Find(indexName).Columns.Should().Contain(columnName);

		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithIndexes(indexes =>
				{
					indexes.Remove(indexName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(0);
	}

	[Fact]
	public void Index_RemoveNonExisting()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "text";
		const string defaultValue = "test default value";
		const string indexName = "idx_hash";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddTextColumn(columnId, columnName, column =>
						{
							column
								.WithDefaultValue(defaultValue)
								.NotNullable();
						});
				})
				.WithIndexes(indexes =>
				{
					indexes
						.AddHashIndex(indexName, index =>
						{
							index.WithColumn(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(1);
		table.Indexes.Find(indexName).Should().NotBeNull();
		table.Indexes.Find(indexName).Should().BeOfType<DbHashIndex>();
		table.Indexes.Find(indexName).Columns.Should().Contain(columnName);

		var nonExistingIndexName = indexName + "123";
		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithIndexes(indexes =>
				{
					indexes.Remove(nonExistingIndexName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"Index with name '{nonExistingIndexName}' is not found.");
	}

	[Fact]
	public void Index_WithSameName()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "text";
		const string defaultValue = "test default value";
		const string indexName = "idx_hash";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddTextColumn(columnId, columnName, column =>
						{
							column
								.WithDefaultValue(defaultValue)
								.NotNullable();
						});
				})
				.WithIndexes(indexes =>
				{
					indexes
						.AddHashIndex(indexName, index =>
						{
							index.WithColumn(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Indexes.Count().Should().Be(1);
		table.Indexes.Find(indexName).Should().NotBeNull();
		table.Indexes.Find(indexName).Should().BeOfType<DbHashIndex>();
		table.Indexes.Find(indexName).Columns.Should().Contain(columnName);

		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithIndexes(indexes =>
				{
					indexes
						.AddHashIndex(indexName, index =>
						{
							index.WithColumn(columnName);
						});
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"There is already existing database object with name '{indexName}'");
	}

	#endregion

	#region <--- Constraints Tests --->

	[Fact]
	public void Constraint_PrimaryKey_CRUD()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "id";
		Guid defaultValue = Guid.NewGuid();
		const string constraintName = "pk_id";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddGuidColumn(columnId, columnName, column =>
						{
							column
								.WithAutoDefaultValue()
								.NotNullable();
						});
				})
				.WithConstraints(constraints =>
				{
					constraints
						.AddPrimaryKeyConstraint(constraintName, constraint =>
						{
							constraint.WithColumns(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Constraints.Count().Should().Be(1);
		table.Constraints.Find(constraintName).Should().NotBeNull();
		table.Constraints.Find(constraintName).Should().BeOfType<DbPrimaryKeyConstraint>();
		table.Constraints.Find(constraintName).Columns.Should().Contain(columnName);

		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithConstraints(constraints =>
				{
					constraints.Remove(constraintName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Constraints.Count().Should().Be(0);
	}

	[Fact]
	public void Constraint_UniqueKey_CRUD()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "id";
		Guid defaultValue = Guid.NewGuid();
		const string constraintName = "ux_id";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddGuidColumn(columnId, columnName, column =>
						{
							column
								.WithAutoDefaultValue()
								.NotNullable();
						});
				})
				.WithConstraints(constraints =>
				{
					constraints
						.AddUniqueKeyConstraint(constraintName, contraint =>
						{
							contraint.WithColumns(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Constraints.Count().Should().Be(1);
		table.Constraints.Find(constraintName).Should().NotBeNull();
		table.Constraints.Find(constraintName).Should().BeOfType<DbUniqueKeyConstraint>();
		table.Constraints.Find(constraintName).Columns.Should().Contain(columnName);

		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithConstraints(constraints =>
				{
					constraints.Remove(constraintName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Constraints.Count().Should().Be(0);
	}

	[Fact]
	public void Constraint_ForeignKey_CRUD()
	{
		var databaseBuilder = DatabaseBuilder.New();

		var data1Builder = databaseBuilder
			.NewTableBuilder(Guid.NewGuid(), "data1")
			.WithColumns(columns =>
			{
				columns.AddGuidColumn("id", column => { column.NotNullable().WithAutoDefaultValue(); });
			});

		var data2Builder = databaseBuilder
			.NewTableBuilder(Guid.NewGuid(), "data2")
			.WithColumns(columns =>
			{
				columns.AddGuidColumn("id", column => { column.NotNullable().WithAutoDefaultValue(); });
			});

		var dataRelBuilder = databaseBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_rel")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("data1_id", column => { column.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("data2_id", column => { column.NotNullable().WithAutoDefaultValue(); });
			});

		var task = Task.Run(() =>
		{
			data1Builder
				.WithConstraints(constraints =>
				{
					constraints
						.AddForeignKeyConstraint("fk_data1_data_rel", constraint =>
						{
							constraint
								.WithColumns("id")
								.WithForeignTable("data_rel")
								.WithForeignColumns("data1_id");
						});
				})
				.Build();

			data2Builder
				.WithConstraints(constraints =>
				{
					constraints
						.AddForeignKeyConstraint("fk_data2_data_rel", constraint =>
						{
							constraint
								.WithColumns("id")
								.WithForeignTable("data_rel")
								.WithForeignColumns("data2_id");
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		var data1Table = data1Builder.Build();

		data1Table.Constraints.Count().Should().Be(1);
		data1Table.Constraints.Find("fk_data1_data_rel").Should().NotBeNull();
		data1Table.Constraints.Find("fk_data1_data_rel").Should().BeOfType<DbForeignKeyConstraint>();
		((DbForeignKeyConstraint)data1Table.Constraints.Find("fk_data1_data_rel")).Columns.Count().Should().Be(1);
		((DbForeignKeyConstraint)data1Table.Constraints.Find("fk_data1_data_rel")).Columns.Should().Contain("id");
		((DbForeignKeyConstraint)data1Table.Constraints.Find("fk_data1_data_rel")).ForeignColumns.Should().Contain("data1_id");
		((DbForeignKeyConstraint)data1Table.Constraints.Find("fk_data1_data_rel")).ForeignTable.Should().Be("data_rel");

		var data2Table = data2Builder.Build();
		data2Table.Constraints.Count().Should().Be(1);
		data2Table.Constraints.Find("fk_data2_data_rel").Should().NotBeNull();
		data2Table.Constraints.Find("fk_data2_data_rel").Should().BeOfType<DbForeignKeyConstraint>();
		((DbForeignKeyConstraint)data2Table.Constraints.Find("fk_data2_data_rel")).Columns.Count().Should().Be(1);
		((DbForeignKeyConstraint)data2Table.Constraints.Find("fk_data2_data_rel")).Columns.Should().Contain("id");
		((DbForeignKeyConstraint)data2Table.Constraints.Find("fk_data2_data_rel")).ForeignColumns.Should().Contain("data2_id");
		((DbForeignKeyConstraint)data2Table.Constraints.Find("fk_data2_data_rel")).ForeignTable.Should().Be("data_rel");

		task = Task.Run(() =>
		{
			data1Builder
				.WithConstraints(constraints =>
				{
					constraints.Remove("fk_data1_data_rel");
				});

			data2Builder
				.WithConstraints(constraints =>
				{
					constraints.Remove("fk_data2_data_rel");
				});
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		data1Builder.Build().Constraints.Count().Should().Be(0);
		data2Builder.Build().Constraints.Count().Should().Be(0);
	}

	[Fact]
	public void Constraint_RemoveNonExisting()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "text";
		const string defaultValue = "test default value";
		const string constraintName = "ux";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddTextColumn(columnId, columnName, column =>
						{
							column
								.WithDefaultValue(defaultValue)
								.NotNullable();
						});
				})
				.WithConstraints(constraints =>
				{
					constraints
						.AddUniqueKeyConstraint(constraintName, index =>
						{
							index.WithColumns(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Constraints.Count().Should().Be(1);
		table.Constraints.Find(constraintName).Should().NotBeNull();
		table.Constraints.Find(constraintName).Should().BeOfType<DbUniqueKeyConstraint>();
		table.Constraints.Find(constraintName).Columns.Should().Contain(columnName);

		var nonExistingConstraintName = constraintName + "123";
		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithConstraints(constraints =>
				{
					constraints.Remove(nonExistingConstraintName);
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"Constraint with name '{nonExistingConstraintName}' is not found.");
	}

	[Fact]
	public void Constraint_WithSameName()
	{
		var databaseBuilder = DatabaseBuilder.New();
		var tableBuilder = CreateEmptyTableBuilder(databaseBuilder);

		Guid columnId = Guid.NewGuid();
		const string columnName = "text";
		const string defaultValue = "test default value";
		const string constraintName = "ux";

		DbTable table = null;
		var task = Task.Run(() =>
		{
			table = tableBuilder
				.WithColumns(columns =>
				{
					columns
						.AddTextColumn(columnId, columnName, column =>
						{
							column
								.WithDefaultValue(defaultValue)
								.NotNullable();
						});
				})
				.WithConstraints(constraints =>
				{
					constraints
						.AddUniqueKeyConstraint(constraintName, constraint =>
						{
							constraint.WithColumns(columnName);
						});
				})
				.Build();
		});
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();

		table.Constraints.Count().Should().Be(1);
		table.Constraints.Find(constraintName).Should().NotBeNull();
		table.Constraints.Find(constraintName).Should().BeOfType<DbUniqueKeyConstraint>();
		table.Constraints.Find(constraintName).Columns.Should().Contain(columnName);

		task = Task.Run(() =>
		{
			table = tableBuilder
				.WithConstraints(constraints =>
				{
					constraints
						.AddUniqueKeyConstraint(constraintName, constraint =>
						{
							constraint.WithColumns(columnName);
						});
				})
				.Build();
		});
		exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().NotBeNull();
		exception.Should().BeOfType<DbBuilderException>();
		exception.Message.Should().Be($"There is already existing database object with name '{constraintName}'");
	}

	#endregion

	#region <--- utility--->

	private DbTableBuilder CreateEmptyTableBuilder(DatabaseBuilder databaseBuilder)
	{
		Guid tableId = Guid.NewGuid();
		string tableName = "test_table";
		Guid tableAppId = Guid.NewGuid();
		Guid tableDpId = Guid.NewGuid();
		return databaseBuilder
			.NewTableBuilder(tableId, tableName)
			.WithApplicationId(tableAppId)
			.WithDataProviderId(tableDpId);
	}

	#endregion


}
