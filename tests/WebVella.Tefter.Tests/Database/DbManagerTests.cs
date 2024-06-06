namespace WebVella.Tefter.Tests.Database;

public partial class DbManagerTests : BaseTest
{
    [Fact]
    public async Task TemplateTest()
    {
        using (await locker.LockAsync())
        {
            IDbService dbService = ServiceProvider.GetRequiredService<IDbService>();
            IDbManager dbManager = ServiceProvider.GetRequiredService<IDbManager>();

            using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
            {
                const string tableName = "test_table";
                Guid appId = Guid.NewGuid();
                Guid dpId = Guid.NewGuid();

                var buildTable = DbTableBuilder
                    .New(tableName)
                    .WithDataProviderId(dpId)
                    .WithApplicationId(appId)
                    .WithColumns(columns =>
                    {
                        columns

                        #region <--- Id --->
                        .AddNewTableIdColumn()
                        #endregion

                        #region <--- AutoInc --->
                        .AddNewAutoIncrementColumn("auto_inc1")
                        .AddNewAutoIncrementColumn("auto_inc2")
                        #endregion

                        #region <--- Guid --->
                        .AddNewGuidColumn("guid_nullable_with_auto_default", c =>
                        {
                            c.WithAutoDefaultValue()
                            .Nullable();
                        })
                        .AddNewGuidColumn("guid_not_nullable_with_auto_default", c =>
                        {
                            c.WithAutoDefaultValue()
                            .NotNullable();
                        })
                        .AddNewGuidColumn("guid_nullable_with_default_value", c =>
                        {
                            c.WithDefaultValue(Guid.NewGuid())
                            .Nullable();
                        })
                        .AddNewGuidColumn("guid_not_nullable_with_default_value", c =>
                        {
                            c.WithDefaultValue(Guid.NewGuid())
                            .NotNullable();
                        })
                        .AddNewGuidColumn("guid_nullable_with_default_value_and_auto", c =>
                        {
                            c.Nullable()
                            .WithDefaultValue(Guid.NewGuid())
                            .WithNoAutoDefaultValue();
                        })
                        .AddNewGuidColumn("guid_not_nullable_with_default_value_and_auto", c =>
                        {
                            c.WithDefaultValue(Guid.NewGuid())
                            .WithNoAutoDefaultValue();
                        })
                        .AddNewGuidColumn("guid_nullable_without_default_value", c => { c.Nullable(); })
                        //this should provide exception
                        //.AddNewGuidColumn("guid_not_nullable_without_default_value", c => { c.Nullable(); }) 
                        #endregion

                        #region <--- Boolean --->
                        .AddNewBooleanColumn("bool_not_nullable_with_default_true", c =>
                        {
                            c.WithDefaultValue(true);
                        })
                        .AddNewBooleanColumn("bool_not_nullable_with_default_false", c =>
                        {
                            c.WithDefaultValue(false);
                        })
                        .AddNewBooleanColumn("bool_nullable_default_true", c =>
                        {
                            c.WithDefaultValue(true).Nullable();
                        })
                        .AddNewBooleanColumn("bool_nullable_default_false", c =>
                        {
                            c.WithDefaultValue(true).Nullable();
                        })
                        .AddNewBooleanColumn("bool_nullable_without_default", c =>
                        {
                            c.Nullable();
                        })
                        //this should provide exception
                        //.AddNewBooleanColumn("bool_not_nullable_without_default", c => { c.NotNullable(); })  
                        #endregion

                        #region <--- Number --->
                        .AddNewNumberColumn("number_nullable_with_default", c =>
                        {
                            c.WithDefaultValue(2)
                            .Nullable();
                        })
                        .AddNewNumberColumn("number_not_nullable_with_default", c =>
                        {
                            c.WithDefaultValue(1).NotNullable();
                        })
                        .AddNewNumberColumn("number_nullable_without_default", c =>
                        {
                            c.Nullable();
                        })
                        //this should provide exception
                        //.AddNewNumberColumn("number_not_nullable_without_default", c => { c.NotNullable(); }) 
                        #endregion

                        #region <--- Date --->
                        .AddNewDateColumn("date_nullable_with_auto_default", c =>
                        {
                            c.WithAutoDefaultValue()
                            .Nullable();
                        })
                        .AddNewDateColumn("date_not_nullable_with_auto_default", c =>
                        {
                            c.WithAutoDefaultValue()
                            .NotNullable();
                        })
                        .AddNewDateColumn("date_nullable_with_default_value", c =>
                        {
                            c.WithDefaultValue(new DateOnly(2000, 1, 1))
                            .Nullable();
                        })
                        .AddNewDateColumn("date_not_nullable_with_default_value", c =>
                        {
                            c.WithDefaultValue(new DateOnly(2000, 1, 1))
                            .NotNullable();
                        })
                        .AddNewDateColumn("date_nullable_with_default_value_and_auto", c =>
                        {
                            c.Nullable()
                            .WithDefaultValue(new DateOnly(2000, 1, 1))
                            .WithNoAutoDefaultValue();
                        })
                        .AddNewDateColumn("date_not_nullable_with_default_value_and_auto", c =>
                        {
                            c.WithDefaultValue(new DateOnly(2000, 1, 1))
                            .WithNoAutoDefaultValue();
                        })
                        .AddNewDateColumn("date_nullable_without_default_value", c =>
                        {
                            c.Nullable();
                        })
                        //this should provide exception
                        //.AddNewDateColumn("date_not_nullable_without_default_value", c => { c.NotNullable(); }) 
                        #endregion

                        #region <--- DateTime --->
                        .AddNewDateTimeColumn("datetime_nullable_with_auto_default", c =>
                        {
                            c.WithAutoDefaultValue()
                            .Nullable();
                        })
                        .AddNewDateTimeColumn("datetime_not_nullable_with_auto_default", c =>
                        {
                            c.WithAutoDefaultValue()
                            .NotNullable();
                        })
                        .AddNewDateTimeColumn("datetime_nullable_with_default_value", c =>
                        {
                            c.WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
                            .Nullable();
                        })
                        .AddNewDateTimeColumn("datetime_not_nullable_with_default_value", c =>
                        {
                            c.WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
                            .NotNullable();
                        })
                        .AddNewDateTimeColumn("datetime_nullable_with_default_value_and_auto", c =>
                        {
                            c.Nullable()
                            .WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
                            .WithNoAutoDefaultValue();
                        })
                        .AddNewDateTimeColumn("datetime_not_nullable_with_default_value_and_auto", c =>
                        {
                            c.WithDefaultValue(new DateTime(2000, 1, 1, 13, 12, 11))
                            .WithNoAutoDefaultValue();
                        })
                        .AddNewDateTimeColumn("datetime_nullable_without_default_value", c =>
                        {
                            c.Nullable();
                        })
                        //this should provide exception
                        //.AddNewDateTimeColumn("datetime_not_nullable_without_default_value", c => { c.NotNullable(); }) 
                        #endregion

                        #region <--- Text --->
                        .AddNewTextColumn("test_nullable_with_default", c =>
                        {
                            c.WithDefaultValue("default1")
                            .Nullable();
                        })
                        .AddNewTextColumn("text_not_nullable_with_default", c =>
                        {
                            c.WithDefaultValue("default2")
                            .NotNullable();
                        })
                        .AddNewTextColumn("text_nullable_without_default", c =>
                        {
                            c.Nullable();
                        });
                        //this should provide exception
                        //.AddNewTextColumn("text_not_nullable_without_default", c => { c.NotNullable(); }) 
                        #endregion

                    }).Build();


                return;


                //var table = new DbTable { Name = "test" };
                //var idColumn = table.AddTableIdColumn();
                //var guid1Column = table.AddGuidColumn("guid1", false, Guid.NewGuid());
                //var guid2Column = table.AddGuidColumn("guid2", false, true);
                //var autoIncColumn = table.AddAutoIncrementColumn("inc");
                //var boolColumn = table.AddBooleanColumn("bool", false, true);
                //var numberColumn = table.AddNumberColumn("number", true, 10);
                //var date1Column = table.AddDateColumn("date1", false, new DateOnly(2024, 10, 25));
                //var date2Column = table.AddDateColumn("date2", false, useCurrentTimeAsDefaultValue: true);
                //var dt1Column = table.AddDateTimeColumn("datetime1", true, DateTime.Now);
                //var dt2Column = table.AddDateTimeColumn("datetime2", true, useCurrentTimeAsDefaultValue: true);
                //var text1Column = table.AddTextColumn("text1", false, "default value 1");
                //var text2Column = table.AddTextColumn("text2", false, "default value 2");

                //var pkCtt = table.AddPrimaryKeyContraint();
                //var ux1Ctt = table.AddUniqueContraint("ux_unique1", "number", "bool", "date1");
                //var ux2Ctt = table.AddUniqueContraint("ux_unique2", "number");

                //var btreeIdx = table.AddBTreeIndex("idx_btree", "bool", "number");
                //var hashIdx = table.AddHashIndex("idx_hash", "date1");
                //var gistIdx = table.AddGistIndex("idx_gist", "text1", "text2");
                //var ginIdx = table.AddGinIndex("idx_gin", "text1", "text2");

                //dbManager.SaveTable(table);

                //List<DbTable> tables = dbManager.LoadTables();
                //var tableAfterSaveFromDb = tables.SingleOrDefault(x => x.Name == table.Name);
                //tableAfterSaveFromDb.Should().NotBeNull();

                //CompareTables(table, tableAfterSaveFromDb);

            }
        }
    }

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
}
