using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Csv.Addons;
using WebVella.Tefter.DataProviders.Csv.Tests.Utils;

namespace WebVella.Tefter.DataProviders.Csv.Tests;

public class CsvDataProviderUtilityTests
{
    #region << MemoryStream >>

    [Fact]
    public void CheckCsvFile_MemoryStream_Success()
    {
        var fileName = "1000-rows.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo, delimiterFound) =
            new CsvDataProviderUtility().CheckCsvFile(inputContext, filepath: fileName,
                provider: new CsvDataProvider());
        isSuccess.Should().BeTrue();
        message.Should().Be("File check successful");
        schemaInfo.Should().NotBeNull();
        delimiterFound.Should().Be(",");
    }

    [Fact]
    public void CheckCsvFile_MemoryStream_Empty()
    {
        var fileName = "empty.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, message, schemaInfo, delimiterFound) =
            new CsvDataProviderUtility().CheckCsvFile(inputContext, filepath: fileName,
                provider: new CsvDataProvider());
        isSuccess.Should().BeFalse();
        message.Should().Be("File content is empty");
        schemaInfo.Should().BeNull();
        delimiterFound.Should().Be(null);
    }

    [Fact]
    public void CheckCsvFile_MemoryStream_NonCsv()
    {
        var fileName = "image-as-csv.csv";
        MemoryStream inputContext = new CsvDataProviderTestUtility().LoadFileStream(fileName);
        var (isSuccess, _, schemaInfo, delimiterFound) =
            new CsvDataProviderUtility().CheckCsvFile(inputContext, filepath: fileName,
                provider: new CsvDataProvider());
        isSuccess.Should().BeFalse();
        schemaInfo.Should().BeNull();
        delimiterFound.Should().Be(null);
    }

    #endregion


    #region << Import Context >>

    [Fact]
    public void CheckCsvFile_Success()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("1000-rows.csv");
        var (result,schema) = new CsvDataProviderUtility().CheckCsvFile(inputContext);
        result.Should().BeTrue();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        progressLog.Should().NotBeNull();
        progressLog.Should().NotBeEmpty();
        schema.Should().NotBeNull();

    }

    [Fact]
    public void CheckCsvFile_Success2()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("csv-no-header.csv");
        var (result,schema) = new CsvDataProviderUtility().CheckCsvFile(inputContext);
        result.Should().BeFalse();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        progressLog.Should().NotBeNull();
        progressLog.Should().NotBeEmpty();
        var hasMessage = progressLog.Any(x =>
            x.Message == "Cannot evaluate the file schema. Error: A header row is required for the file");
        hasMessage.Should().BeTrue();
        schema.Should().BeNull();
       
    }

    [Fact]
    public void CheckCsvFile_Failure()
    {
        var inputContext = new CsvDataProviderTestUtility().CreateFileContext("image-as-csv.csv");
        var (result,schema) = new CsvDataProviderUtility().CheckCsvFile(inputContext);
        result.Should().BeFalse();
        var progressLog = inputContext.ProcessStream.GetProgressLog();
        progressLog.Should().NotBeNull();
        progressLog.Should().NotBeEmpty();
        var errorMessage = progressLog.FirstOrDefault(x =>
            x.Message == "[CsvDataProvider] File contents contain non printable or invalid characters");
        errorMessage.Should().BeNull();
        schema.Should().BeNull();
        
    }

    #endregion
}