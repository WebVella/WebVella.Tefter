using System.Collections.ObjectModel;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Tests.Utility;

public class SearchParserTests
{
	#region << 	GeneratePostgresSearchScript >>

	[Fact]
	public void SearchParserTestsTest_0()
	{
		string input = "plumb";
		var sqlParams = new List<NpgsqlParameter>();
		var sql = input.GeneratePostgresSearchScript("tf_search", sqlParams);
		sqlParams.Count.Should().Be(1);
		sql.Should().Be($"tf_search ILIKE CONCAT ('%', @search_param_1, '%')");
	}

	[Fact]
	public void SearchParserTestsTest_1()
	{
		string input = "plumb OR pair";
		var sqlParams = new List<NpgsqlParameter>();
		var sql = input.GeneratePostgresSearchScript("tf_search", sqlParams);
		sqlParams.Count.Should().Be(2);
		sql.Should()
			.Be(
				"tf_search ILIKE CONCAT ('%', @search_param_1, '%') OR tf_search ILIKE CONCAT ('%', @search_param_2, '%')");
	}

	[Fact]
	public void SearchParserTestsTest_2()
	{
		string input = "(apple AND pears) OR plumb";
		var sqlParams = new List<NpgsqlParameter>();
		var sql = input.GeneratePostgresSearchScript("tf_search", sqlParams);
		sqlParams.Count.Should().Be(3);
		sql.Should()
			.Be(
				"(tf_search ILIKE CONCAT ('%', @search_param_1, '%') AND tf_search ILIKE CONCAT ('%', @search_param_2, '%')) OR tf_search ILIKE CONCAT ('%', @search_param_3, '%')");
	}

	[Fact]
	public void SearchParserTestsTest_3()
	{
		string input = "(plumb OR pair)";
		var sqlParams = new List<NpgsqlParameter>();
		var sql = input.GeneratePostgresSearchScript("tf_search", sqlParams);
		sqlParams.Count.Should().Be(2);
		sql.Should()
			.Be(
				"(tf_search ILIKE CONCAT ('%', @search_param_1, '%') OR tf_search ILIKE CONCAT ('%', @search_param_2, '%'))");
	}

	[Fact]
	public void SearchParserTestsTest_3a()
	{
		string input = "((plumb OR pair))";
		var sqlParams = new List<NpgsqlParameter>();
		var sql = input.GeneratePostgresSearchScript("tf_search", sqlParams);
		sqlParams.Count.Should().Be(2);
		sql.Should()
			.Be(
				"((tf_search ILIKE CONCAT ('%', @search_param_1, '%') OR tf_search ILIKE CONCAT ('%', @search_param_2, '%')))");
	}

	[Fact]
	public void SearchParserTestsTest_4()
	{
		string input = "(plumb OR pair) AND (plumb or apple) AND strawberry";
		var sqlParams = new List<NpgsqlParameter>();
		var sql = input.GeneratePostgresSearchScript("tf_search", sqlParams);
		sqlParams.Count.Should().Be(5);
		sql.Should()
			.Be(
				"(tf_search ILIKE CONCAT ('%', @search_param_1, '%') OR tf_search ILIKE CONCAT ('%', @search_param_2, '%')) AND (tf_search ILIKE CONCAT ('%', @search_param_3, '%') OR tf_search ILIKE CONCAT ('%', @search_param_4, '%')) AND tf_search ILIKE CONCAT ('%', @search_param_5, '%')");
	}

	[Fact]
	public void SearchParserTestsTest_5()
	{
		string input = "(plumb OR (pair AND apple)) AND strawberry";
		var sqlParams = new List<NpgsqlParameter>();
		var sql = input.GeneratePostgresSearchScript("tf_search", sqlParams);
		sqlParams.Count.Should().Be(4);
		sql.Should()
			.Be(
				"(tf_search ILIKE CONCAT ('%', @search_param_1, '%') OR (tf_search ILIKE CONCAT ('%', @search_param_2, '%') AND tf_search ILIKE CONCAT ('%', @search_param_3, '%'))) AND tf_search ILIKE CONCAT ('%', @search_param_4, '%')");
	}

	#endregion
	
	#region << 	GeneratePostgresSearchScript >>

	[Fact]
	public void JoinUserSearchQueriesTest_AllNull()
	{
		string? query1 = null;
		string? query2 = null;
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().BeNull();
	}	
	
	[Fact]
	public void JoinUserSearchQueriesTest_FirstNull()
	{
		string? query1 = null;
		string? query2 = "test2";
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"{query2}");
	}	
	
	[Fact]
	public void JoinUserSearchQueriesTest_SecondNull()
	{
		string? query1 = "test1";
		string? query2 = null;
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"{query1}");
	}
	
	[Fact]
	public void JoinUserSearchQueriesTest_BothNotNull()
	{
		string? query1 = "test1";
		string? query2 = "test2";
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"{query1} AND {query2}");
	}	
	
	[Fact]
	public void JoinUserSearchQueriesTest_WithAND()
	{
		string? query1 = "test1";
		string? query2 = "test2 AND test2";
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"{query1} AND ({query2})");
	}	
	
	[Fact]
	public void JoinUserSearchQueriesTest_BothLogical()
	{
		string? query1 = "test1 OR test1";
		string? query2 = "test2 AND test2";
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"({query1}) AND ({query2})");
	}	
	
	[Fact]
	public void JoinUserSearchQueriesTest_Square()
	{
		string? query1 = "(test1)";
		string? query2 = null;
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"{query1}");
	}	
	
	[Fact]
	public void JoinUserSearchQueriesTest_SquareAndLogical()
	{
		string? query1 = "(test1 AND test1)";
		string? query2 = null;
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"{query1}");
	}		

	[Fact]
	public void JoinUserSearchQueriesTest_SquareAndLogicalComplexSurroundedByBrackets()
	{
		string? query1 = "((test1 AND test1) OR (test1 AND test1))";
		string? query2 = "test2";
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"{query1} AND {query2}");
	}
	
	[Fact]
	public void JoinUserSearchQueriesTest_SquareAndLogicalComplexNotSurroundedByBrackets()
	{
		string? query1 = "(test1 AND test1) OR (test1 AND test1)";
		string? query2 = "test2";
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"({query1}) AND {query2}");
	}				
	
	[Fact]
	public void JoinUserSearchQueriesTest_SquareAndLogicalComplex2()
	{
		string? query1 = "(test1 AND test1) OR (test1 AND test1)";
		string? query2 = "test2 OR test1";
		var result = query1.JoinUserSearchQueries(query2);
		result.Should().Be($"({query1}) AND ({query2})");
	}		
	
	#endregion
	
	#region << ValidateOperatorParentheses >>
	[Fact]
	public void ValidateOperatorParenthesesTests()
	{
// Scenario 1: Simple true case
		bool result = "(test)".ValidateOperatorParentheses();
		result.Should().BeTrue(); 

		// Scenario 2: Invalid OR placement
		result = "test OR test".ValidateOperatorParentheses();
		result.Should().BeFalse();

		// Scenario 3: Valid nested AND
		result = "(test and test)".ValidateOperatorParentheses();
		result.Should().BeTrue();

		// Scenario 4: Invalid combined expression
		result = "(test and test) or (test and test)".ValidateOperatorParentheses();
		result.Should().BeFalse();

		// Scenario 5: Valid nested with OR
		result = "((test and test) or (test and test))".ValidateOperatorParentheses();
		result.Should().BeTrue();

		// Scenario 6: Standalone invalid operator
		result = "or".ValidateOperatorParentheses();
		result.Should().BeFalse();

		// Scenario 7: Valid single OR in parentheses
		result = "(OR)".ValidateOperatorParentheses();
		result.Should().BeTrue();
	}		
	#endregion
}