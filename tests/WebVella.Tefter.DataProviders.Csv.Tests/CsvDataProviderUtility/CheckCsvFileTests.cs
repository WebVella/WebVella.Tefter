using System.Text;
using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Csv.Tests.Utils;

namespace WebVella.Tefter.DataProviders.Csv.Tests;

public class CsvDataProviderUtilityTests
{
    #region << MemoryStream >>
    [Fact]
    public async Task CheckCsvFile_MemoryStream_Success()
    {
        var fileName = "100-rows.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message) = await inputContext.CheckCsvFile(filepath:fileName);
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
    }    
    
    [Fact]
    public async Task CheckCsvFile_MemoryStream_Empty()
    {
        var fileName = "empty.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message) = await inputContext.CheckCsvFile(filepath:fileName);
        isSuccess.Should().BeFalse();
        message.Should().Be("File content is empty");
    }      
    
    [Fact]
    public async Task CheckCsvFile_MemoryStream_NonCsv()
    {
        var fileName = "image-as-csv.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message) = await inputContext.CheckCsvFile(filepath:fileName);
        isSuccess.Should().BeFalse();
    }          
    #endregion

  
    
    #region << Import Context >>
    [Fact]
    public async Task CheckCsvFile_Success()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("csv-header.csv");
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
        inputContext.IsSuccess.Should().BeTrue();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        progressLog.Should().NotBeNull();
        progressLog.Should().NotBeEmpty();
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