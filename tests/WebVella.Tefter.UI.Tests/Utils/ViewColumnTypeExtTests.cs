using WebVella.Tefter.Database;

namespace WebVella.Tefter.UI.Tests.Utils;
public class ViewColumnTypeExtTests
{
	[Fact]
	public void NoRules_Should_Provider_EmptyResult()
	{
		//Given
		Guid rowId = Guid.NewGuid();
		string columnName = "test";
		string queryName = "query";
		Guid columnValue = Guid.NewGuid();		
		var (dt,rules,queryNameToColumnNameDict) = GetSample1(rowId, columnName,queryName, columnValue);
		
		rules = new();
		//When
		var result = dt.GenerateColoring(rowId,rules, queryNameToColumnNameDict);
		//Than
		result.Should().BeEmpty();
	}
	
	[Fact]
	public void NoMappingDict_Should_Provider_EmptyResult()
	{
		//Given
		Guid rowId = Guid.NewGuid();
		string columnName = "test";
		string queryName = "query";
		Guid columnValue = Guid.NewGuid();		
		var (dt,rules,queryNameToColumnNameDict) = GetSample1(rowId, columnName,queryName, columnValue);
		queryNameToColumnNameDict = new();
		//When
		var result = dt.GenerateColoring(rowId,rules, queryNameToColumnNameDict);
		//Than
		result.Should().BeEmpty();
	}	

	[Fact]
	public void LogicalRules_Should_Not_BeApplied()
	{
		//Given
		Guid rowId = Guid.NewGuid();
		string columnName = "test";
		string queryName = "query";
		Guid columnValue = Guid.NewGuid();		
		var (dt,rules,queryNameToColumnNameDict) = GetSample1(rowId, columnName,queryName, columnValue);
		rules = new List<TfColoringRule>()
		{
			new TfColoringRule()
			{
				ForegroundColor = TfColor.Amber500,
				BackgroundColor = TfColor.Amber500,
				Columns = new(),
				Position = 1,
				Filters = new List<TfFilterQuery>()
				{
					new()
					{
						Name = new TfFilterAnd().GetColumnName(),
						Items =
						{
							new TfFilterQuery()
							{
								Name = queryName,
								Method = (int)TfFilterTextComparisonMethod.Equal,
								Value = columnValue.ToString(),
							}
						}
					}
				}
			}
		};		
		//When
		var result = dt.GenerateColoring(rowId,rules, queryNameToColumnNameDict);
		//Than
		result.Should().BeEmpty();
	}	
	
	[Fact]
	public void Row_SimpleRule_Should_Be_BeApplied()
	{
		//Given
		Guid rowId = Guid.NewGuid();
		string columnName = "test";
		string queryName = "query";
		Guid columnValue = Guid.NewGuid();		
		var (dt,rules,queryNameToColumnNameDict) = GetSample1(rowId, columnName,queryName, columnValue);
		
		//When
		var result = dt.GenerateColoring(rowId,rules, queryNameToColumnNameDict);
		//Than
		result.Should().NotBeEmpty();
		result.Keys.Should().Contain(rowId.ToString());
	}


	private (TfDataTable,List<TfColoringRule>,Dictionary<string, string>) GetSample1(
		Guid rowId, 
		string columnName, 
		string queryName, 
		Guid columnValue)
	{
		var dt = new TfDataTable();
		var rules = new List<TfColoringRule>();
		var queryNameToColumnNameDict = new Dictionary<string, string>();
		
		//Datatable
		dt.Columns.Add(
			new TfDataColumn(
				dataTable:dt,
				name: TfConstants.TEFTER_ITEM_ID_PROP_NAME,
				dbType: TfDatabaseColumnType.Guid,
				isNullable:false,
				origin: TfDataColumnOriginType.CurrentProvider			
			));		
		dt.Columns.Add(
			new TfDataColumn(
				dataTable:dt,
				name: columnName,
				dbType: TfDatabaseColumnType.Guid,
				isNullable:true,
				origin: TfDataColumnOriginType.CurrentProvider			
			));
		dt.Rows.Add(new TfDataRow(dt,new object[]
		{
			rowId,
			columnValue
		}));		
		
		//rules
		rules.Add(new TfColoringRule()
		{
			ForegroundColor = TfColor.Amber500,
			BackgroundColor = TfColor.Amber500,
			Columns = new(),
			Position = 1,
			Filters = new List<TfFilterQuery>()
			{
				new TfFilterQuery()
				{
					Name = queryName,
					Method = (int)TfFilterTextComparisonMethod.Equal,
					Value = columnValue.ToString(),
				}
			}
		});
		
		//mapping
		queryNameToColumnNameDict[queryName] = columnName;
		
		
		return (dt,rules,queryNameToColumnNameDict);
	}
}
