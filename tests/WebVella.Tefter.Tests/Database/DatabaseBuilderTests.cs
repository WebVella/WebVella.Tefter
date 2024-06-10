namespace WebVella.Tefter.Tests.Database;

public partial class DatabaseBuilderTests : BaseTest
{
	[Fact]
	public async Task CreateTable()
	{
		using (await locker.LockAsync())
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
	}

	#region <--- old code --->
	//private void CompareTables(DbTable table1, DbTable table2)
	//{
	//    table1.Should().NotBeNull();
	//    table1.Name.Should().Be(table2.Name);
	//    table1.Columns.Count().Should().Be(table2.Columns.Count());
	//    table1.Indexes.Count().Should().Be(table2.Indexes.Count());
	//    table1.Constraints.Count().Should().Be(table2.Constraints.Count());

	//    for (int i = 0; i < table1.Columns.Count(); i++)
	//    {
	//        var column1 = table1.Columns[i];
	//        var column2 = table2.Columns[i];

	//        column1.GetType().Should().Be(column2.GetType());

	//        if (column1.GetType() == typeof(DbGuidColumn))
	//            ((DbGuidColumn)column1).GenerateNewIdAsDefaultValue.Should().Be(
	//                ((DbGuidColumn)column2).GenerateNewIdAsDefaultValue);
	//        if (column1.GetType() == typeof(DbDateColumn))
	//            ((DbDateColumn)column1).UseCurrentTimeAsDefaultValue.Should().Be(
	//                ((DbDateColumn)column2).UseCurrentTimeAsDefaultValue);
	//        if (column1.GetType() == typeof(DbDateTimeColumn))
	//            ((DbDateTimeColumn)column1).UseCurrentTimeAsDefaultValue.Should().Be(
	//                ((DbDateTimeColumn)column2).UseCurrentTimeAsDefaultValue);

	//        column1.Name.Should().Be(column2.Name);
	//        column1.DefaultValue.Should().Be(column2.DefaultValue);
	//        column1.IsNullable.Should().Be(column2.IsNullable);
	//    }

	//    for (int i = 0; i < table1.Indexes.Count(); i++)
	//    {
	//        var index1 = table1.Indexes[i];
	//        var index2 = table2.Indexes.SingleOrDefault(x => x.Name == index1.Name);

	//        index1.GetType().Should().Be(index2.GetType());
	//        index1.Table.Name.Should().Be(index2.Table.Name);
	//        index1.Columns.Count().Should().Be(index2.Columns.Count());
	//        index1.Name.Should().Be(index2.Name);

	//        for (int j = 0; j < index1.Columns.Count(); j++)
	//        {
	//            var column1 = index1.Columns[j];
	//            var column2 = index2.Columns[j];
	//            column1.Table.Name.Should().Be(column2.Table.Name);
	//            column1.GetType().Should().Be(column2.GetType());
	//        }
	//    }

	//    for (int i = 0; i < table1.Constraints.Count(); i++)
	//    {
	//        var constraint1 = table1.Constraints[i];
	//        var constraint2 = table2.Constraints.SingleOrDefault(x => x.Name == constraint1.Name);

	//        constraint1.GetType().Should().Be(constraint2.GetType());
	//        constraint1.Name.Should().Be(constraint2.Name);
	//        constraint1.Table.Name.Should().Be(constraint2.Table.Name);
	//        constraint1.Columns.Count().Should().Be(constraint2.Columns.Count());

	//        for (int j = 0; j < constraint1.Columns.Count(); j++)
	//        {
	//            var column1 = constraint1.Columns[j];
	//            var column2 = constraint2.Columns[j];
	//            column1.Table.Name.Should().Be(column2.Table.Name);
	//            column1.GetType().Should().Be(column2.GetType());
	//        }
	//    }
	//} 
	#endregion
}
