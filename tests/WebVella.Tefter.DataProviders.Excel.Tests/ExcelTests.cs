using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Excel.Addons;

namespace WebVella.Tefter.DataProviders.Excel.Tests;

public class UnitTest1
{
    [Fact]
    public void CheckDataParse()
    {
        var fileName = "data.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();        
    }
}