using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Csv.Addons;
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
       
        var hasMessage = progressLog.Any(x =>
            x.Message == "File contents contain non printable or invalid characters");
        hasMessage.Should().BeTrue();
    }   
    
    [Fact]
    public async Task ParseCsvFile_WithHeader()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("1000-rows.csv");
        await inputContext.CreateFromCsvFile();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        inputContext.IsSuccess.Should().BeTrue();
        
    }    
    
   
    
}