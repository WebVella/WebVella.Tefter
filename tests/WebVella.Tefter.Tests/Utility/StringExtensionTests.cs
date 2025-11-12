using System.Collections.ObjectModel;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Tests.Utility;

public class StringExtensionTests
{
	
	[Fact]
	public void ConvertFileNameToDataProviderNameTest()
	{
		Dictionary<string, string> input = new()
		{
			{String.Empty,"unknown"},
			{" ","unknown"},
			{"test","test"},
			{"1","1"},
			{"file.xls","file.xls"},
			{" file.xls ","file.xls"},
		};

		foreach (var key in input.Keys)
		{
			var result = key.ConvertFileNameToDataProviderName();
			result.Should().BeEquivalentTo(input[key]);
		}

	}
}