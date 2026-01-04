using AwesomeAssertions;
using WebVella.Tefter.Database;
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
        schemaInfo.SourceColumnDefaultDbType["text"].Should().Be(TfDatabaseColumnType.Text);
        schemaInfo.SourceColumnDefaultDbType["number"].Should().Be(TfDatabaseColumnType.LongInteger);
        schemaInfo.SourceColumnDefaultDbType["date"].Should().Be(TfDatabaseColumnType.DateOnly);
        schemaInfo.SourceColumnDefaultDbType["datetime"].Should().Be(TfDatabaseColumnType.DateTime);
        schemaInfo.SourceColumnDefaultDbType["boolean"].Should().Be(TfDatabaseColumnType.Boolean);
        schemaInfo.SourceColumnDefaultDbType["guid"].Should().Be(TfDatabaseColumnType.Guid);
        schemaInfo.SourceColumnDefaultDbType["currency"].Should().Be(TfDatabaseColumnType.Number);
        schemaInfo.SourceColumnDefaultDbType["percents"].Should().Be(TfDatabaseColumnType.Number);
        schemaInfo.SourceColumnDefaultDbType["accounting"].Should().Be(TfDatabaseColumnType.Number);
    }
    
    [Fact]
    public void CheckDataParse2()
    {
        var fileName = "industrial-hub-fixed.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();
        schemaInfo.SourceColumnDefaultDbType["Index"].Should().Be(TfDatabaseColumnType.Text);
        schemaInfo.SourceColumnDefaultDbType["Group"].Should().Be(TfDatabaseColumnType.Text);
        schemaInfo.SourceColumnDefaultDbType["Subgroup"].Should().Be(TfDatabaseColumnType.Text);
        schemaInfo.SourceColumnDefaultDbType["Activity"].Should().Be(TfDatabaseColumnType.Text);
        schemaInfo.SourceColumnDefaultDbType["Q-ty"].Should().Be(TfDatabaseColumnType.LongInteger);
        schemaInfo.SourceColumnDefaultDbType["Unit"].Should().Be(TfDatabaseColumnType.Text);
        schemaInfo.SourceColumnDefaultDbType["Rate"].Should().Be(TfDatabaseColumnType.Number);
        schemaInfo.SourceColumnDefaultDbType["Total"].Should().Be(TfDatabaseColumnType.Number);
        schemaInfo.SourceColumnDefaultDbType["Manufacturer"].Should().Be(TfDatabaseColumnType.Text);
    }    
}