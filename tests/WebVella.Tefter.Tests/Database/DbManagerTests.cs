namespace WebVella.Tefter.Tests.Database;

public partial class DbManagerTests : BaseTest
{
    [Fact]
    public async Task TemplateTest()
    {
        using (await locker.LockAsync())
        {
            IDbService dbService = Context.Services.GetService<IDbService>();

   
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
