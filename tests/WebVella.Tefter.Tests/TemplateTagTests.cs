namespace WebVella.Tefter.Tests;
using ClosedXML.Excel;
using WebVella.Tefter.Models;
using WebVella.Tefter.Utility;

public class TemplateTagTestsBase
{
	private TfDataTable _data;
	public TemplateTagTestsBase()
	{
		var ds = new TfDataTable();
		ds.Columns.Add(new TfDataColumn(ds, "position", TfDatabaseColumnType.Integer, false, false, false));
		ds.Columns.Add(new TfDataColumn(ds, "sku", TfDatabaseColumnType.Text, false, false, false));
		ds.Columns.Add(new TfDataColumn(ds, "name", TfDatabaseColumnType.Text, false, false, false));
		ds.Columns.Add(new TfDataColumn(ds, "price", TfDatabaseColumnType.Number, false, false, false));
		var values = new List<Tuple<int, string, string, decimal>>();
		for (int i = 0; i < 5; i++)
		{
			var position = i + 1;
			values.Add(new Tuple<int, string, string, decimal>(position, $"sku{position}", $"item{position}", (decimal)(position * 10)));
		}

		foreach (var item in values)
		{
			var dsrow = new TfDataRow(ds, new object[ds.Columns.Count]);
			dsrow["position"] = item.Item1;
			dsrow["sku"] = item.Item2;
			dsrow["name"] = item.Item3;
			dsrow["price"] = item.Item4;
			ds.Rows.Add(dsrow);
		}
		_data = ds;
	}
	public TfDataTable SampleData => _data;
}

public partial class TemplateTagTests : TemplateTagTestsBase
{
	#region << DATA >>
	[Fact]
	public void NullTemplateShouldReturnNoTags()
	{
		//Given
		string template = "";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(0);
	}

	[Fact]
	public void EmptyTemplateShouldReturnNoTags()
	{
		//Given
		string template = string.Empty;
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateShouldReturnOneDataTag()
	{
		//Given
		var columnName = "column_name";
		string template = "{{" + columnName + "}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].IndexList.Should().NotBeNull();
		result[0].IndexList.Count.Should().Be(0);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateShouldReturnOneDataTagAlwaysLowecase()
	{
		//Given
		var columnName = "ColumnName";
		string template = "{{" + columnName + "}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName.ToLowerInvariant());
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateShouldReturnOneDataTag_WithSpaces()
	{
		//Given
		var columnName = "columnName";
		string template = " {{" + columnName + "}} ";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template.Trim());
		result[0].Name.Should().Be(columnName.ToLowerInvariant());
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateShouldReturnOneDataTag_WithSpaces2()
	{
		//Given
		var columnName = "column_name";
		string template = "{{ " + columnName + " }}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateShouldReturnOneDataTag_WithSpaces3()
	{
		//Given
		var columnName = "column_name";
		string template = "{{  " + columnName + "     }}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void OnInTextTemplateShouldReturnOneDataTag()
	{
		//Given
		var columnName = "column_name";
		string template = "this is {{" + columnName + "}} a test";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be("{{" + columnName + "}}");
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void OnInTextTemplateShouldReturnOneDataTagWithOneParam()
	{
		//Given
		var columnName = "column_name";
		string template = "this is {{" + columnName + "(\"test\")}} a test";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be("{{" + columnName + "(\"test\")}}");
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters[0].Name.Should().BeNull();
		result[0].ParamGroups[0].Parameters[0].Value.Should().Be("test");
	}

	[Fact]
	public void OnInTextTemplateShouldReturnOneDataTagWithOneNamedParam()
	{
		//Given
		var columnName = "column_name";
		var paramName = "param1";
		var paramValue = "test";
		string template = "{{" + columnName + "(" + paramName + "=\"" + paramValue + "\")}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters[0].Name.Should().Be(paramName.ToLowerInvariant());
		result[0].ParamGroups[0].Parameters[0].Value.Should().Be(paramValue);
	}

	[Fact]
	public void OnInTextTemplateShouldReturnOneDataTagWithOneNamedParamWithSpaces()
	{
		//Given
		var columnName = "column_name";
		var paramName = "param1";
		var paramValue = "test";
		string template = "{{" + columnName + " ( " + paramName + " = \"" + paramValue + "\" )}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters[0].Name.Should().Be(paramName);
		result[0].ParamGroups[0].Parameters[0].Value.Should().Be(paramValue);
	}

	[Fact]
	public void OnInTextTemplateShouldReturnOneDataTagWithOneNamedParamShouldBeLowered()
	{
		//Given
		var columnName = "column_name";
		var paramName = "TestParam";
		var paramValue = "test";
		string template = "{{" + columnName + "(" + paramName + "=\"" + paramValue + "\")}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].ParamGroups.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters.Count.Should().Be(1);
		result[0].ParamGroups[0].Parameters[0].Name.Should().Be(paramName.ToLowerInvariant());
		result[0].ParamGroups[0].Parameters[0].Value.Should().Be(paramValue);
	}

	[Fact]
	public void ExactTemplateShouldReturnMultipleDataTag()
	{
		//Given
		var columnName = "column_name";
		var columnName2 = "column_name2";
		string template = "{{" + columnName + "}}{{" + columnName2 + "}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(2);
		var column1Tag = result.FirstOrDefault(x => x.Name == columnName);
		var column2Tag = result.FirstOrDefault(x => x.Name == columnName2);

		column1Tag.FullString.Should().Be("{{" + columnName + "}}");
		column1Tag.Name.Should().Be(columnName);
		column1Tag.Type.Should().Be(TfTemplateTagType.Data);
		column1Tag.ParamGroups.Count.Should().Be(0);

		column2Tag.FullString.Should().Be("{{" + columnName2 + "}}");
		column2Tag.Name.Should().Be(columnName2);
		column2Tag.Type.Should().Be(TfTemplateTagType.Data);
		column2Tag.ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateShouldReturnMultipleDataTagWithParams()
	{
		//Given
		var columnName = "column_name";
		var columnNameParam1Name = "Param1";
		var columnNameParam1Value = "Param =1 Value";
		var columnNameParam2Name = "Param2";
		var columnNameParam2Value = "Param2Valu";
		var columnName2 = "column_name2";
		var columnName2Param1Name = "Param1";
		var columnName2Param1Value = "Param1Value";
		var columnName2Param2Name = "Param2";
		var columnName2Param2Value = "Param2Valu";
		string template = "{{" + columnName + "(" + columnNameParam1Name + "=\"" + columnNameParam1Value + "\", " + columnNameParam2Name + "=\"" + columnNameParam2Value + "\")}}" +
		"{{" + columnName2 + "(" + columnName2Param1Name + "=\"" + columnName2Param1Value + "\", " + columnName2Param2Name + "=\"" + columnName2Param2Value + "\")}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(2);
		var column1Tag = result.FirstOrDefault(x => x.Name == columnName);
		var column2Tag = result.FirstOrDefault(x => x.Name == columnName2);

		column1Tag.FullString.Should().Be("{{" + columnName + "(" + columnNameParam1Name + "=\"" + columnNameParam1Value + "\", " + columnNameParam2Name + "=\"" + columnNameParam2Value + "\")}}");
		column1Tag.Name.Should().Be(columnName);
		column1Tag.Type.Should().Be(TfTemplateTagType.Data);
		column1Tag.ParamGroups.Count.Should().Be(1);
		column1Tag.ParamGroups[0].Parameters.Count.Should().Be(2);
		var column1Param1 = column1Tag.ParamGroups[0].Parameters.FirstOrDefault(x => x.Name == columnNameParam1Name.ToLowerInvariant());
		var column1Param2 = column1Tag.ParamGroups[0].Parameters.FirstOrDefault(x => x.Name == columnNameParam2Name.ToLowerInvariant());
		column1Param1.Should().NotBeNull();
		column1Param2.Should().NotBeNull();
		column1Param1.Value.Should().Be(columnNameParam1Value);
		column1Param2.Value.Should().Be(columnNameParam2Value);

		column2Tag.FullString.Should().Be("{{" + columnName2 + "(" + columnName2Param1Name + "=\"" + columnName2Param1Value + "\", " + columnName2Param2Name + "=\"" + columnName2Param2Value + "\")}}");
		column2Tag.Name.Should().Be(columnName2);
		column2Tag.Type.Should().Be(TfTemplateTagType.Data);
		column2Tag.ParamGroups.Count.Should().Be(1);
		column2Tag.ParamGroups[0].Parameters.Count.Should().Be(2);
		var column2Param1 = column2Tag.ParamGroups[0].Parameters.FirstOrDefault(x => x.Name == columnName2Param1Name.ToLowerInvariant());
		var column2Param2 = column2Tag.ParamGroups[0].Parameters.FirstOrDefault(x => x.Name == columnName2Param2Name.ToLowerInvariant());
		column2Param1.Should().NotBeNull();
		column2Param2.Should().NotBeNull();
		column2Param1.Value.Should().Be(columnName2Param1Value);
		column2Param2.Value.Should().Be(columnName2Param2Value);
	}

	[Fact]
	public void ExactTemplateShouldReturnMultipleDataTagIntext()
	{
		//Given
		var columnName = "column_name";
		var columnName2 = "column_name2";
		string template = "test is with {{" + columnName + "}} and longer text {{" + columnName2 + "}} everywhere";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(2);
		var column1Tag = result.FirstOrDefault(x => x.Name == columnName);
		var column2Tag = result.FirstOrDefault(x => x.Name == columnName2);
		column1Tag.Should().NotBeNull();
		column2Tag.Should().NotBeNull();
		column1Tag.FullString.Should().Be("{{" + columnName + "}}");
		column1Tag.Name.Should().Be(columnName);
		column1Tag.Type.Should().Be(TfTemplateTagType.Data);
		column1Tag.ParamGroups.Count.Should().Be(0);

		column2Tag.FullString.Should().Be("{{" + columnName2 + "}}");
		column2Tag.Name.Should().Be(columnName2);
		column2Tag.Type.Should().Be(TfTemplateTagType.Data);
		column2Tag.ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateShouldReturnEmptyGroups()
	{
		//Given
		var columnName = "column_name";
		string template = "{{" + columnName + "()(\"test\")(test=1)}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		var column1Tag = result.FirstOrDefault(x => x.Name == columnName);
		column1Tag.Should().NotBeNull();
		column1Tag.FullString.Should().Be("{{" + columnName + "()(\"test\")(test=1)}}");
		column1Tag.Name.Should().Be(columnName);
		column1Tag.Type.Should().Be(TfTemplateTagType.Data);
		column1Tag.ParamGroups.Count.Should().Be(3);
		column1Tag.ParamGroups[0].FullString.Should().Be("()");
		column1Tag.ParamGroups[0].Parameters.Count.Should().Be(0);
		column1Tag.ParamGroups[1].FullString.Should().Be("(\"test\")");
		column1Tag.ParamGroups[1].Parameters.Count.Should().Be(1);
		column1Tag.ParamGroups[2].FullString.Should().Be("(test=1)");
		column1Tag.ParamGroups[2].Parameters.Count.Should().Be(1);
	}

	[Fact]
	public void ExactTemplateShouldReturnOneTagWithMultipleParameterGroups()
	{
		//Given
		var columnName = "column_name";
		var columnNameParam1Name = "Param1";
		var columnNameParam1Value = "Param =1 Value";
		var columnNameParam2Name = "Param2";
		var columnNameParam2Value = "Param2Valu";
		string template = "{{" + columnName +
		"(" + columnNameParam1Name + "=\"" + columnNameParam1Value + "\")" +
		"(" + columnNameParam2Name + "=\"" + columnNameParam2Value + "\")" +
		"}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		var column1Tag = result.FirstOrDefault(x => x.Name == columnName);
		column1Tag.Should().NotBeNull();

		column1Tag.FullString.Should().Be("{{column_name(Param1=\"Param =1 Value\")(Param2=\"Param2Valu\")}}");
		column1Tag.Name.Should().Be(columnName);
		column1Tag.Type.Should().Be(TfTemplateTagType.Data);
		column1Tag.ParamGroups.Count.Should().Be(2);
		column1Tag.ParamGroups[0].Parameters.Count.Should().Be(1);
		column1Tag.ParamGroups[1].Parameters.Count.Should().Be(1);


		column1Tag.ParamGroups[0].Parameters[0].Name.Should().Be(columnNameParam1Name.ToLowerInvariant());
		column1Tag.ParamGroups[0].Parameters[0].Value.Should().Be(columnNameParam1Value);

		column1Tag.ParamGroups[1].Parameters[0].Name.Should().Be(columnNameParam2Name.ToLowerInvariant());
		column1Tag.ParamGroups[1].Parameters[0].Value.Should().Be(columnNameParam2Value);
	}

	[Fact]
	public void ExactTemplateWithIndexingShouldReturnOneDataTagAndIndex()
	{
		//Given
		var columnName = "column_name";
		int columnIndex = 5;
		string template = "{{" + columnName + "[" + columnIndex + "]}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].IndexList.Should().NotBeNull();
		result[0].IndexList.Count.Should().Be(1);
		result[0].IndexList[0].Should().Be(columnIndex);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateWithIndexingShouldReturnOneDataTagAndIndexZERO()
	{
		//Given
		var columnName = "column_name";
		int columnIndex = 5;
		string template = "{{" + columnName + "[][" + columnIndex + "]}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].IndexList.Should().NotBeNull();
		result[0].IndexList.Count.Should().Be(2);
		result[0].IndexList[0].Should().Be(0);
		result[0].IndexList[1].Should().Be(columnIndex);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	[Fact]
	public void ExactTemplateWithMultiIndexingShouldReturnOneDataTagAndMultiIndex()
	{
		//Given
		var columnName = "column_name";
		int columnIndex = 5;
		int columnIndex2 = 2;
		string template = "{{" + columnName + "[" + columnIndex + "][" + columnIndex2 + "]}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(columnName);
		result[0].Type.Should().Be(TfTemplateTagType.Data);
		result[0].IndexList.Should().NotBeNull();
		result[0].IndexList.Count.Should().Be(2);
		result[0].IndexList[0].Should().Be(columnIndex);
		result[0].IndexList[1].Should().Be(columnIndex2);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	#endregion

	#region << FUNCTION >>
	[Fact]
	public void ExactTemplateShouldReturnOneFunctionTag()
	{
		//Given
		var functionName = "functionName";
		string template = "{{=" + functionName + "()}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(functionName.ToLowerInvariant());
		result[0].Type.Should().Be(TfTemplateTagType.Function);
		result[0].ParamGroups.Count.Should().Be(1);
	}
	[Fact]
	public void ExactTemplateShouldReturnOneFunctionTagWithMissingParams()
	{
		//Given
		var functionName = "functionName";
		string template = "{{=" + functionName + "}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(functionName.ToLowerInvariant());
		result[0].Type.Should().Be(TfTemplateTagType.Function);
		result[0].ParamGroups.Count.Should().Be(0);
	}

	#endregion

	#region << EXCEL FUNCTION >>
	[Fact]
	public void ExactTemplateShouldReturnOneExcelFunctionTag()
	{
		//Given
		var functionName = "functionName";
		string template = "{{==" + functionName + "()}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(functionName.ToLowerInvariant());
		result[0].Type.Should().Be(TfTemplateTagType.ExcelFunction);
		result[0].ParamGroups.Count.Should().Be(1);
	}
	[Fact]
	public void ExactTemplateShouldReturnOneExcelFunctionTagWithMissingParams()
	{
		//Given
		var functionName = "functionName";
		string template = "{{==" + functionName + "}}";
		//When
		List<TfTemplateTag> result = TfTemplateUtility.GetTagsFromTemplate(template);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(1);
		result[0].FullString.Should().Be(template);
		result[0].Name.Should().Be(functionName.ToLowerInvariant());
		result[0].Type.Should().Be(TfTemplateTagType.ExcelFunction);
		result[0].ParamGroups.Count.Should().Be(0);
	}
	#endregion

	#region << Processing >>
	[Fact]
	public void TemplateProcessShouldReturnNoResultsIfEmpty()
	{
		//Given
		string template = "";
		TfDataTable ds = SampleData;
		//When
		List<TfTemplateTagResult> result = TfTemplateUtility.ProcessTemplateTag(template, ds);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(5);
		result[0].Value.Should().Be(template);
	}

	[Fact]
	public void TemplateProcessShouldReturnNoResultsIfNoTag()
	{
		//Given
		string template = "sometext test";
		TfDataTable ds = SampleData;
		//When
		List<TfTemplateTagResult> result = TfTemplateUtility.ProcessTemplateTag(template, ds);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(5);
		result[0].Value.Should().Be(template);
	}

	[Fact]
	public void TemplateProcessShouldReturnResultsIfTagCannotBeProcessed()
	{
		//Given
		string template = "{{}}";
		TfDataTable ds = SampleData;
		//When
		List<TfTemplateTagResult> result = TfTemplateUtility.ProcessTemplateTag(template, ds);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(5);
		result[0].Value.Should().Be(template);
	}

	[Fact]
	public void TemplateProcessShouldReturnResultsIfTagCanBeProcessed()
	{
		//Given
		string template = "{{name}}";
		TfDataTable ds = SampleData;
		//When
		List<TfTemplateTagResult> result = TfTemplateUtility.ProcessTemplateTag(template, ds);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(5);
		result[0].Value.Should().Be(ds.Rows[0]["name"]?.ToString());
		result[1].Value.Should().Be(ds.Rows[1]["name"]?.ToString());
		result[2].Value.Should().Be(ds.Rows[2]["name"]?.ToString());
		result[3].Value.Should().Be(ds.Rows[3]["name"]?.ToString());
		result[4].Value.Should().Be(ds.Rows[4]["name"]?.ToString());
	}

	[Fact]
	public void TemplateProcessShouldReturnResultsIfTagCanBeProcessedMulti()
	{
		//Given
		string template = "{{position}}.{{name}}";
		TfDataTable ds = SampleData;
		//When
		List<TfTemplateTagResult> result = TfTemplateUtility.ProcessTemplateTag(template, ds);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(5);
		result[0].Value.Should().Be(ds.Rows[0]["position"] + "." + ds.Rows[0]["name"]);
		result[1].Value.Should().Be(ds.Rows[1]["position"] + "." + ds.Rows[1]["name"]);
		result[2].Value.Should().Be(ds.Rows[2]["position"] + "." + ds.Rows[2]["name"]);
		result[3].Value.Should().Be(ds.Rows[3]["position"] + "." + ds.Rows[3]["name"]);
		result[4].Value.Should().Be(ds.Rows[4]["position"] + "." + ds.Rows[4]["name"]);

	}

	[Fact]
	public void TemplateProcessShouldReturnResultsIfTagCanBeProcessedMultiFixedIndex()
	{
		//Given
		string template = "{{position}}.{{name[0]}}";
		TfDataTable ds = SampleData;
		//When
		List<TfTemplateTagResult> result = TfTemplateUtility.ProcessTemplateTag(template, ds);
		//Then
		result.Should().NotBeNull();
		result.Count.Should().Be(5);
		result[0].Value.Should().Be(ds.Rows[0]["position"] + "." + ds.Rows[0]["name"]);
		result[1].Value.Should().Be(ds.Rows[1]["position"] + "." + ds.Rows[0]["name"]);
		result[2].Value.Should().Be(ds.Rows[2]["position"] + "." + ds.Rows[0]["name"]);
		result[3].Value.Should().Be(ds.Rows[3]["position"] + "." + ds.Rows[0]["name"]);
		result[4].Value.Should().Be(ds.Rows[4]["position"] + "." + ds.Rows[0]["name"]);

	}


	#endregion
}
