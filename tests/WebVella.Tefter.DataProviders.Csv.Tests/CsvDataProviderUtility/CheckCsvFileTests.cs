using System.Text;
using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Csv.Addons;
using WebVella.Tefter.DataProviders.Csv.Tests.Utils;

namespace WebVella.Tefter.DataProviders.Csv.Tests;

public class CsvDataProviderUtilityTests
{
    #region << MemoryStream >>
    [Fact]
    public async Task CheckCsvFile_MemoryStream_Success()
    {
        var fileName = "1000-rows.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message,schemaInfo) = await inputContext.CheckCsvFile(filepath:fileName, provider:new CsvDataProvider());
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();
    }    
    
    [Fact]
    public async Task CheckCsvFile_MemoryStream_Empty()
    {
        var fileName = "empty.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message,schemaInfo) = await inputContext.CheckCsvFile(filepath:fileName, provider:new CsvDataProvider());
        isSuccess.Should().BeFalse();
        message.Should().Be("File content is empty");
        schemaInfo.Should().BeNull();
    }      
    
    [Fact]
    public async Task CheckCsvFile_MemoryStream_NonCsv()
    {
        var fileName = "image-as-csv.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message,schemaInfo) = await inputContext.CheckCsvFile(filepath:fileName, provider:new CsvDataProvider());
        isSuccess.Should().BeFalse();
        schemaInfo.Should().BeNull();
    }          
    #endregion

  
    
    #region << Import Context >>
    [Fact]
    public async Task CheckCsvFile_Success()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("1000-rows.csv");
        await inputContext.CheckCsvFile();
        inputContext.IsSuccess.Should().BeTrue();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        progressLog.Should().NotBeNull();
        progressLog.Should().NotBeEmpty();
    }
    [Fact]
    public async Task CheckCsvFile_Success2()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("csv-no-header.csv");
        await inputContext.CheckCsvFile();
        inputContext.IsSuccess.Should().BeFalse();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        progressLog.Should().NotBeNull();
        progressLog.Should().NotBeEmpty();
        var hasMessage = progressLog.Any(x =>
            x.Message == "Cannot evaluate the file schema. Error: A header row is required for the file");
        hasMessage.Should().BeTrue();
    }    
    [Fact]
    public async Task CheckCsvFile_Failure()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("image-as-csv.csv");
        await inputContext.CheckCsvFile();
        inputContext.IsSuccess.Should().BeFalse();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        progressLog.Should().NotBeNull();
        progressLog.Should().NotBeEmpty();
        var errorMessage = progressLog.FirstOrDefault(x =>
            x.Message == "[CsvDataProvider] File contents contain non printable or invalid characters");
        errorMessage.Should().BeNull();
    }  
    #endregion
}