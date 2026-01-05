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
        
        schemaInfo.SourceColumnDefaultSourceType["text"].Should().Be("TEXT");
        schemaInfo.SourceColumnDefaultSourceType["number"].Should().Be("NUMBER");
        schemaInfo.SourceColumnDefaultSourceType["date"].Should().Be("DATETIME");
        schemaInfo.SourceColumnDefaultSourceType["datetime"].Should().Be("DATETIME");
        schemaInfo.SourceColumnDefaultSourceType["boolean"].Should().Be("BOOLEAN");
        schemaInfo.SourceColumnDefaultSourceType["guid"].Should().Be("GUID");
        schemaInfo.SourceColumnDefaultSourceType["currency"].Should().Be("NUMBER");
        schemaInfo.SourceColumnDefaultSourceType["percents"].Should().Be("NUMBER");
        schemaInfo.SourceColumnDefaultSourceType["accounting"].Should().Be("NUMBER");      
        
        schemaInfo.SourceColumnDefaultValue["text"].Should().Be("Tempor feugait dolor sit ut gubergren lorem tempor sea. Erat labore magna aliquyam delenit et. Dolore dolor erat ea ut et tempor duo.");
        schemaInfo.SourceColumnDefaultValue["number"].Should().Be("1");
        schemaInfo.SourceColumnDefaultValue["date"].Should().Be("1/12/2024");
        schemaInfo.SourceColumnDefaultValue["datetime"].Should().Be("12/11/2024 10:32:00 AM");
        schemaInfo.SourceColumnDefaultValue["boolean"].Should().Be("True");
        schemaInfo.SourceColumnDefaultValue["guid"].Should().Be("d2fcbf38-2a4a-4b92-b9e7-823abc4ca900");
        schemaInfo.SourceColumnDefaultValue["currency"].Should().Be("15.25");
        schemaInfo.SourceColumnDefaultValue["percents"].Should().Be("0.25");
        schemaInfo.SourceColumnDefaultValue["accounting"].Should().Be("15.25");       
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);
    }
    
    [Fact]
    public void CheckDataEdgeCase1()
    {
        var fileName = "edge-case1.xlsx";
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
        
        schemaInfo.SourceColumnDefaultSourceType["Index"].Should().Be("TEXT");
        schemaInfo.SourceColumnDefaultSourceType["Group"].Should().Be("TEXT");
        schemaInfo.SourceColumnDefaultSourceType["Subgroup"].Should().Be("TEXT");
        schemaInfo.SourceColumnDefaultSourceType["Activity"].Should().Be("TEXT");
        schemaInfo.SourceColumnDefaultSourceType["Q-ty"].Should().Be("NUMBER");
        schemaInfo.SourceColumnDefaultSourceType["Unit"].Should().Be("TEXT");
        schemaInfo.SourceColumnDefaultSourceType["Rate"].Should().Be("NUMBER");
        schemaInfo.SourceColumnDefaultSourceType["Total"].Should().Be("NUMBER");
        schemaInfo.SourceColumnDefaultSourceType["Manufacturer"].Should().Be("TEXT");        
        
        schemaInfo.SourceColumnDefaultValue["Index"].Should().Be("52.01.01");
        schemaInfo.SourceColumnDefaultValue["Group"].Should().Be("Group 1");
        schemaInfo.SourceColumnDefaultValue["Subgroup"].Should().Be("SubGroup 1");
        schemaInfo.SourceColumnDefaultValue["Activity"].Should().Be("Activity 1");
        schemaInfo.SourceColumnDefaultValue["Q-ty"].Should().Be("12");
        schemaInfo.SourceColumnDefaultValue["Unit"].Should().Be("m3");
        schemaInfo.SourceColumnDefaultValue.Should().NotContainKey("Rate");
        schemaInfo.SourceColumnDefaultValue.Should().NotContainKey("Total");
        schemaInfo.SourceColumnDefaultValue.Should().NotContainKey("Manufacturer");
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);        
    }    
 [Fact]
    public void CheckDataEdgeCase2()
    {
        var fileName = "edge-case2.xlsx";
        MemoryStream inputContext = new TestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo) =
            new ExcelDataProviderUtility().CheckExcelFile(inputContext, filepath: fileName,
                provider: new ExcelDataProvider());        
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();
        schemaInfo.SourceColumnDefaultDbType["Index"].Should().Be(TfDatabaseColumnType.DateOnly);
        
        schemaInfo.SourceColumnDefaultSourceType["Index"].Should().Be("DATETIME");
        
        schemaInfo.SourceColumnDefaultValue["Index"].Should().Be("2.01.01");
        
        schemaInfo.SourceTypeSupportedDbTypes.Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["TEXT"].Count().Should().Be(3);
        schemaInfo.SourceTypeSupportedDbTypes["NUMBER"].Count().Should().Be(4);
        schemaInfo.SourceTypeSupportedDbTypes["DATETIME"].Count().Should().Be(2);
        schemaInfo.SourceTypeSupportedDbTypes["BOOLEAN"].Count().Should().Be(1);
        schemaInfo.SynchPrimaryKeyColumns.Count().Should().Be(0);        
    }        
}