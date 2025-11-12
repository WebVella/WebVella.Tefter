using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Csv.Tests.Utils;

namespace WebVella.Tefter.DataProviders.Csv.Tests;

public class GetHeaderAsyncTests
{
 

    [Fact]
    public void GetHeaderAsync_EmptyCSV()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("empty.csv");
        
        var result = memStream.GetHeaderAsync();
        result.Count.Should().Be(0);
    }   
    
    [Fact]
    public void GetHeaderAsync_NoHeaderCSV()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("csv-no-header.csv");
        
        var result = memStream.GetHeaderAsync();
        result.Count.Should().Be(0);
    }       
    
    [Fact]
    public void GetHeaderAsync_HasHeader()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("1000-rows.csv");
        var result = memStream.GetHeaderAsync();
        result.Count.Should().Be(9);
    }       
     
    
}