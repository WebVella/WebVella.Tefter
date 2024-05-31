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
                table.AddDateColumn("date", false,null,useCurrentTimeAsDefaultValue:true);
                table.AddDateTimeColumn("datetime", true, DateTime.Now);
                table.AddTextColumn("text", false, "default value");

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
