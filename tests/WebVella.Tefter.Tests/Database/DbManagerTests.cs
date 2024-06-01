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
            
            using (var scope = dbService.CreateTransactionScope())
            {
                var table = new DbTable();
                table.Name = "test";
                table.AddTableIdColumn();
                table.AddAutoIncrementColumn("inc");
                table.AddBooleanColumn("bool", false, true);
                table.AddNumberColumn("number", true, 10);
                table.AddDateColumn("date1", false, new DateOnly(2024, 10, 25));
                table.AddDateColumn("date2", false,null,useCurrentTimeAsDefaultValue:true);
                table.AddDateTimeColumn("datetime1", true, DateTime.Now);
                table.AddDateTimeColumn("datetime2", true, useCurrentTimeAsDefaultValue: true);
                table.AddTextColumn("text1", false, "default value 1");
                table.AddTextColumn("text2", false, "default value 2");

                table.AddPrimaryKeyContraint("tefter_id");
                table.AddUniqueContraint("ux_unique1", "number", "bool", "date1");
                table.AddUniqueContraint("ux_unique2", "number" );

                table.AddBTreeIndex("idx_btree", "bool", "number");
                table.AddHashIndex("idx_hash", "date1" );
                table.AddGistIndex("idx_gist", "text1", "text2" );
                table.AddGinIndex("idx_gin", "text1", "text2");


                dbManager.SaveTable(table);

                scope.Complete();
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
