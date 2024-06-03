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
                var table = new DbTable { Name = "test"};
                var idColumn = table.AddTableIdColumn();
                var guid1Column = table.AddGuidColumn("guid1", false, Guid.NewGuid());
                var guid2Column = table.AddGuidColumn("guid2", false, true);
                var autoIncColumn = table.AddAutoIncrementColumn("inc");
                var boolColumn = table.AddBooleanColumn("bool", false, true);
                var numberColumn = table.AddNumberColumn("number", true, 10);
                var date1Column = table.AddDateColumn("date1", false, new DateOnly(2024, 10, 25));
                var date2Column = table.AddDateColumn("date2", false, useCurrentTimeAsDefaultValue: true);
                var dt1Column = table.AddDateTimeColumn("datetime1", true, DateTime.Now);
                var dt2Column = table.AddDateTimeColumn("datetime2", true, useCurrentTimeAsDefaultValue: true);
                var text1Column = table.AddTextColumn("text1", false, "default value 1");
                var text2Column = table.AddTextColumn("text2", false, "default value 2");

                var pkCtt = table.AddPrimaryKeyContraint("id");
                var ux1Ctt = table.AddUniqueContraint("ux_unique1", "number", "bool", "date1");
                var ux2Ctt = table.AddUniqueContraint("ux_unique2", "number");

                var btreeIdx = table.AddBTreeIndex("idx_btree", "bool", "number");
                var hashIdx = table.AddHashIndex("idx_hash", "date1");
                var gistIdx = table.AddGistIndex("idx_gist", "text1", "text2");
                var ginIdx = table.AddGinIndex("idx_gin", "text1", "text2");

                dbManager.SaveTable(table);
              

                List<DbTable> tables = dbManager.LoadTables();
                var loadedTable = tables[0];    
                loadedTable.Should().NotBeNull();
                loadedTable.Name.Should().Be(table.Name);
                loadedTable.Columns.Count().Should().Be(table.Columns.Count());
                loadedTable.Indexes.Count().Should().Be(table.Indexes.Count());
                loadedTable.Constraints.Count().Should().Be(table.Constraints.Count());

                for ( int i = 0; i < table.Columns.Count(); i++ )
                {
                    var column = table.Columns[i];
                    var loadedColumn = loadedTable.Columns[i];

                    column.GetType().Should().Be(loadedColumn.GetType());
                   
                    if( column.GetType() == typeof(DbGuidColumn) )
                        ((DbGuidColumn)column).GenerateNewIdAsDefaultValue.Should().Be(
                            ((DbGuidColumn)loadedColumn).GenerateNewIdAsDefaultValue);
                    if (column.GetType() == typeof(DbDateColumn))
                        ((DbDateColumn)column).UseCurrentTimeAsDefaultValue.Should().Be(
                            ((DbDateColumn)loadedColumn).UseCurrentTimeAsDefaultValue);
                    if (column.GetType() == typeof(DbDateTimeColumn))
                        ((DbDateTimeColumn)column).UseCurrentTimeAsDefaultValue.Should().Be(
                            ((DbDateTimeColumn)loadedColumn).UseCurrentTimeAsDefaultValue);

                    column.Name.Should().Be(loadedColumn.Name);
                    column.DefaultValue.Should().Be(loadedColumn.DefaultValue);
                    column.IsNullable.Should().Be(loadedColumn.IsNullable);
                }


                //scope.Complete();
            }

            //var task1 = Task.Run(async () =>
            //{
            //    using (var scope = dbService.CreateTransactionScope())
            //    {
            //    }
            //});

            //var exception1 = Record.ExceptionAsync(async () => await task1).Result;
            //exception1.Should().BeNull();
        }
    }
}
