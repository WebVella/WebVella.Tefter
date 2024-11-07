using WebVella.Tefter.Utility;
using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.Tests;
public class FormulaUtilityTests
{
	[Fact]
	public void CheckFormulaUtilityNowWithEmpty()
	{
		//Given
		string input = "";
		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().Be(null);
	}

	[Fact]
	public void CheckFormulaUtilityNowWithEmptyWithDefault()
	{
		//Given
		string input = "";
		DateTime? defaultDate = new DateTime(2024, 01, 01);
		DateTime? output = null;
		output = input.GetDateFromFormulaString(defaultDate);
		output.Should().Be(defaultDate);

	}
	[Fact]
	public void CheckFormulaUtilityNOW()
	{
		//Given
		string input = "";
		DateTime? output = null;

		input = "NOW()";
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Now;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(now.Hour);
	}

	[Fact]
	public void CheckFormulaUtilityNOW_Lowered()
	{
		//Given
		string input = "now()";
		DateTime? output = null;
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Now;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(now.Hour);
	}

	[Fact]
	public void CheckFormulaUtilityNOW_WithOffsetMinus()
	{
		//Given
		double? offset = -1;
		string input = $"NOW({offset})";

		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);

		var now = DateTime.Now.AddHours(offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(now.Hour);
	}
	[Fact]
	public void CheckFormulaUtilityNOW_WithOffsetMinusFraction()
	{
		//Given
		double? offset = -1.5;
		string input = $"NOW({offset})";
		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Now.AddHours(offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(now.Hour);
	}
	[Fact]
	public void CheckFormulaUtilityNOW_WithOffsetPlusFraction()
	{
		//Given
		double? offset = 1.5;
		string input = $"NOW({offset})";
		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Now.AddHours(offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(now.Hour);
	}

	[Fact]
	public void CheckFormulaUtilityNOW_WithMultiple()
	{
		//Given
		double? offset = 1.5;
		string input = $"NOW({offset})NOW(-5)YEAR(+2)";
		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);

		var now = DateTime.Now.AddHours(offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(now.Hour);
	}

	[Fact]
	public void CheckFormulaUtilityNOW_WithOffsetCommaAndDot()
	{
		//Given
		string input = $"NOW(2.5)";
		string input2 = $"NOW(2,5)";
		DateTime? output = null;
		DateTime? output2 = null;

		output = input.GetDateFromFormulaString();
		output2 = input2.GetDateFromFormulaString();
		output.Should().NotBe(null);
		output2.Should().NotBe(null);

		output.Value.Year.Should().Be(output2.Value.Year);
		output.Value.Month.Should().Be(output2.Value.Month);
		output.Value.Day.Should().Be(output2.Value.Day);
		output.Value.Hour.Should().Be(output2.Value.Hour);
	}

	/*DAY*/

	[Fact]
	public void CheckFormulaUtilityDAY()
	{
		//Given
		string input = "";
		DateTime? output = null;

		input = "DAY()";
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}

	[Fact]
	public void CheckFormulaUtilityDAY_Lowered()
	{
		//Given
		string input = "DAY()";
		DateTime? output = null;
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}
	[Fact]
	public void CheckFormulaUtilityDAY_WithOffsetMinus()
	{
		//Given
		double? offset = -1;
		string input = $"DAY({offset})";

		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);

		var now = DateTime.Today.AddDays(offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}
	[Fact]
	public void CheckFormulaUtilityDAY_WithOffsetMinusFraction()
	{
		//Given
		double? offset = -1.5;
		string input = $"DAY({offset})";
		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today.AddDays(offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(12);
		output.Value.Minute.Should().Be(0);
	}

	/*MONTH*/

	[Fact]
	public void CheckFormulaUtilityMONTH()
	{
		//Given
		string input = "";
		DateTime? output = null;

		input = "MONTH()";
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(1);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}

	[Fact]
	public void CheckFormulaUtilityMONTH_Lowered()
	{
		//Given
		string input = "month()";
		DateTime? output = null;
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(1);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}
	[Fact]
	public void CheckFormulaUtilityMONTH_WithOffsetMinus()
	{
		//Given
		double? offset = 1;
		string input = $"MONTH({offset})";

		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);

		var now = DateTime.Today.AddMonths((int)offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}
	[Fact]
	public void CheckFormulaUtilityMONTH_WithOffsetMinusFraction()
	{
		//Given
		double? offset = 1.5;
		string input = $"MONTH({offset})";
		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today.AddMonths((int)offset.Value);
		var until = DateTime.Today.AddMonths((int)offset.Value + 1);
		output.Value.Should().BeAfter(now);
		output.Value.Should().BeBefore(until);
	}

	/*YEAR*/

	[Fact]
	public void CheckFormulaUtilityYEAR()
	{
		//Given
		string input = "";
		DateTime? output = null;

		input = "YEAR()";
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(1);
		output.Value.Day.Should().Be(1);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}

	[Fact]
	public void CheckFormulaUtilityYEAR_Lowered()
	{
		//Given
		string input = "year()";
		DateTime? output = null;
		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today;
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(1);
		output.Value.Day.Should().Be(1);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}
	[Fact]
	public void CheckFormulaUtilityYEAR_WithOffsetMinus()
	{
		//Given
		double? offset = 1;
		string input = $"YEAR({offset})";

		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);

		var now = DateTime.Today.AddYears((int)offset.Value);
		output.Value.Year.Should().Be(now.Year);
		output.Value.Month.Should().Be(now.Month);
		output.Value.Day.Should().Be(now.Day);
		output.Value.Hour.Should().Be(0);
		output.Value.Minute.Should().Be(0);
	}
	[Fact]
	public void CheckFormulaUtilityYEAR_WithOffsetMinusFraction()
	{
		//Given
		double? offset = 1.5;
		string input = $"YEAR({offset})";
		DateTime? output = null;

		output = input.GetDateFromFormulaString();
		output.Should().NotBe(null);
		var now = DateTime.Today.AddYears((int)offset.Value);
		var until = DateTime.Today.AddYears((int)offset.Value + 1);
		output.Value.Should().BeAfter(now);
		output.Value.Should().BeBefore(until);
	}

	/*Excel*/
	[Fact]
	public void CheckGeneralFormulaParsing()
	{
		//Given
		var idName = "Id";
		var taxName = "tax";
		var priceName = "price";
		var totalName = "total";
		var rowId = Guid.NewGuid();
		DataTable table = new DataTable();

		DataColumn idColumn = new DataColumn();
		idColumn.DataType = System.Type.GetType("System.Guid");
		idColumn.ColumnName = idName;
		idColumn.DefaultValue = Guid.NewGuid();

		DataColumn priceColumn = new DataColumn();
		priceColumn.DataType = System.Type.GetType("System.Decimal");
		priceColumn.ColumnName = priceName;
		priceColumn.DefaultValue = 50;

		// Create the second, calculated, column.
		DataColumn taxColumn = new DataColumn();
		taxColumn.DataType = System.Type.GetType("System.Decimal");
		taxColumn.ColumnName = taxName;
		taxColumn.Expression = $"{priceName} * 1.25";
		

		// Create third column.
		DataColumn totalColumn = new DataColumn();
		totalColumn.DataType = System.Type.GetType("System.Decimal");
		totalColumn.ColumnName = totalName;
		totalColumn.Expression = $"{priceName} + {taxName}";

		// Add columns to DataTable.
		table.Columns.Add(idColumn);
		table.Columns.Add(priceColumn);
		table.Columns.Add(taxColumn);
		table.Columns.Add(totalColumn);


		DataRow _ravi = table.NewRow();
		_ravi[idName] = rowId;
		_ravi[priceName] = 20;
		table.Rows.Add(_ravi);

		//var result = dt.Compute(expression,$"id='{rowId}'");
		//var result = dt.Compute(expression,null);
		DataView view = new DataView(table);
		foreach (DataRow item in table.Rows)
		{
			var test = item.Field<decimal>(totalName);
		}
	}

}
