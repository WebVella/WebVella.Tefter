using AwesomeAssertions;
using WebVella.Tefter.Database;
using WebVella.Tefter.DataProviders.Excel.Addons;

namespace WebVella.Tefter.DataProviders.Excel.Tests;

public class DateTests
{
    [Fact]
    public void CheckDateAsGeneral()
    {
        var fileName = "date-as-general.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();       
        schemaInfo.SourceColumnDefaultDbType["datetime"].Should().Be(TfDatabaseColumnType.Text);
       
        schemaInfo.SourceColumnDefaultSourceType["datetime"].Should().Be("TEXT");
       
        schemaInfo.SourceColumnDefaultValue["datetime"].Should().Be("21.12.01");
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);
    }
    
    [Fact]
    public void CheckDateAsText()
    {
        var fileName = "date-as-text.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();       
        schemaInfo.SourceColumnDefaultDbType["datetime"].Should().Be(TfDatabaseColumnType.Text);
       
        schemaInfo.SourceColumnDefaultSourceType["datetime"].Should().Be("TEXT");
       
        schemaInfo.SourceColumnDefaultValue["datetime"].Should().Be("25.12.01");
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);
    }
    
    [Fact]
    public void CheckDateAsDate()
    {
        var fileName = "date-as-date.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();       
        schemaInfo.SourceColumnDefaultDbType["datetime"].Should().Be(TfDatabaseColumnType.DateTime);
       
        schemaInfo.SourceColumnDefaultSourceType["datetime"].Should().Be("DATETIME");
       
        schemaInfo.SourceColumnDefaultValue["datetime"].Should().Be("12/11/2024 10:32:00 AM");
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);
    }    

    [Fact]
    public void CheckDateAsDateBg()
    {
        var fileName = "date-as-date-bg.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();       
        schemaInfo.SourceColumnDefaultDbType["datetime"].Should().Be(TfDatabaseColumnType.DateTime);
       
        schemaInfo.SourceColumnDefaultSourceType["datetime"].Should().Be("DATETIME");
       
        schemaInfo.SourceColumnDefaultValue["datetime"].Should().Be("12/11/2024 10:32:00 AM");
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);
    }       
    [Fact]
    public void CheckDateAsCustomFormat()
    {
        var fileName = "date-custom-format.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();       
        schemaInfo.SourceColumnDefaultDbType["datetime"].Should().Be(TfDatabaseColumnType.DateTime);
       
        schemaInfo.SourceColumnDefaultSourceType["datetime"].Should().Be("DATETIME");
       
        schemaInfo.SourceColumnDefaultValue["datetime"].Should().Be("12/11/2024 10:32:00 AM");
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);
    }
    
    
    
}