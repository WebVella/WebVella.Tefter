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
			{ String.Empty, "unknown" },
			{ " ", "unknown" },
			{ "test", "test" },
			{ "1", "1" },
			{ "file.xls", "file.xls" },
			{ " file.xls ", "file.xls" },
		};

		foreach (var key in input.Keys)
		{
			var result = key.ConvertFileNameToDataProviderName();
			result.Should().BeEquivalentTo(input[key]);
		}
	}

	[Fact]
	public void GetColumnNameWithoutPrefix_ProviderColumn()
	{
		var inpput = "dp1_name";
		var expected = "name";
		var result = inpput.GetColumnNameWithoutPrefix();
		Assert.Equal(expected,result);
	}
	[Fact]
	public void GetColumnNameWithoutPrefix_ProviderColumn2()
	{
		var inpput = "dp1_column_name";
		var expected = "column_name";
		var result = inpput.GetColumnNameWithoutPrefix();
		Assert.Equal(expected,result);
	}	

	[Fact]
	public void GetColumnNameWithoutPrefix_IdentityColumn()
	{
		var inpput = "identity_name.dp1_name";
		var expected = "identity_name.name";
		var result = inpput.GetColumnNameWithoutPrefix();
		Assert.Equal(expected,result);
	}
	
	[Fact]
	public void GetColumnNameWithoutPrefix_IdentityColumn2()
	{
		var inpput = "identity_name.dp1_column_name";
		var expected = "identity_name.column_name";
		var result = inpput.GetColumnNameWithoutPrefix();
		Assert.Equal(expected,result);
	}	

	[Fact]
	public void GetColumnNameWithoutPrefix_IdentitySharedColumn()
	{
		var inpput = "identity.sc_column";
		var expected = "identity.sc_column";
		var result = inpput.GetColumnNameWithoutPrefix();
		Assert.Equal(expected,result);
	}

	[Fact]
	public void GetColumnNameWithoutPrefix_IdentitySharedColumn2()
	{
		var inpput = "identity.sc_column_name";
		var expected = "identity.sc_column_name";
		var result = inpput.GetColumnNameWithoutPrefix();
		Assert.Equal(expected,result);
	}	
}