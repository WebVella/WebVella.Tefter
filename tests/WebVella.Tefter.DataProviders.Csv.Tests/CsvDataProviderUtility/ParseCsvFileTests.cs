using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Csv.Tests.Utils;

namespace WebVella.Tefter.DataProviders.Csv.Tests;

public class ParseCsvFileTests
{
   
    //Header
    [Fact]
    public async Task ParseCsvFile_Failure()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("image-as-csv.csv");
        await inputContext.CreateFromCsvFile();
        inputContext.IsSuccess.Should().BeFalse();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
       
        var message = progressLog.FirstOrDefault(x =>
            x.Message == "[CsvDataProvider] File contents contain non printable or invalid characters");
        message.Should().NotBeNull();
    }   
    
    [Fact]
    public async Task ParseCsvFile_WithHeader()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("csv-header.csv");
        await inputContext.CreateFromCsvFile();
        inputContext.IsSuccess.Should().BeTrue();
        
    }    
    
    [Fact]
    public async Task ParseCsvFile_NoHeader()
    {
        var inputContext = new CsvDataProviderTestUtility().LoadFileStream("1000-rows.csv");
        var result = await inputContext.InferSchemaAsync();
        
    }        
    
}