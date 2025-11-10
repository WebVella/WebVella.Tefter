using AwesomeAssertions;
using WebVella.Tefter.DataProviders.Csv.Tests.Utils;

namespace WebVella.Tefter.DataProviders.Csv.Tests;

public class GenerateSampleRowIndexHashsetTests
{
 

    [Fact]
    public void GenerateSampleRowIndexHashset_EmptyCSV()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("empty.csv");
        var maxSampleSize = 200;
        var skipCount = 0;
        
        var result = memStream.GenerateSampleRowIndexHashset(maxSampleSize,skipCount);
        result.Count.Should().Be(0);
    }   
    
    [Fact]
    public void GenerateSampleRowIndexHashset_EmptyCSV_WithSkipCount()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("empty.csv");
        var maxSampleSize = 200;
        var skipCount = 5;
        
        var result = memStream.GenerateSampleRowIndexHashset(maxSampleSize,skipCount);
        result.Count.Should().Be(0);
    }      
    
    [Fact]
    public void GenerateSampleRowIndexHashset_EmptyCSV_MaxSampleSize()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("empty.csv");
        var maxSampleSize = 0;
        var skipCount = 0;
        
        var result = memStream.GenerateSampleRowIndexHashset(maxSampleSize,skipCount);
        result.Count.Should().Be(0);
    }       
 
    
    [Fact]
    public void GenerateSampleRowIndexHashset_OneRowHeader_NewLine()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("one-row-only-header-with-new-line.csv");
        var maxSampleSize = 100;
        var skipCount = 0;
        
        var result = memStream.GenerateSampleRowIndexHashset(maxSampleSize,skipCount);
        result.Count.Should().Be(2);
    }          
    
    [Fact]
    public void GenerateSampleRowIndexHashset_OneRowHeader()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("one-row-only-header.csv");
        var maxSampleSize = 100;
        var skipCount = 0;
        
        var result = memStream.GenerateSampleRowIndexHashset(maxSampleSize,skipCount);
        result.Count.Should().Be(1);
    }        
    
    [Fact]
    public void GenerateSampleRowIndexHashset_OneRowHeader2()
    {
        var memStream = new CsvDataProviderTestUtility().LoadFileAsStream("one-row-only-header.csv");
        var maxSampleSize = 100;
        var skipCount = 1;
        
        var result = memStream.GenerateSampleRowIndexHashset(maxSampleSize,skipCount);
        result.Count.Should().Be(0);
    }     
    
}