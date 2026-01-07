using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;


[DboCacheModel]
[TfDboModel("tf_data_provider_column")]
public class TfDataProviderColumn
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[TfDboModelProperty("source_name")]
	public string? SourceName { get; set; }

	[TfDboModelProperty("source_type")]
	public string? SourceType { get; set; } = null;

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; }

	[Required]
	[TfDboModelProperty("db_name")]
	public string? DbName { get; set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDatabaseColumnType>))]
	[TfDboModelProperty("db_type")]
	public TfDatabaseColumnType DbType { get; set; } = TfDatabaseColumnType.Text;

	[TfDboModelProperty("default_value")]
	public string? DefaultValue { get; set; }

	[TfDboModelProperty("auto_default_value")]
	public bool AutoDefaultValue { get; set; } 

	[TfDboModelProperty("is_nullable")]
	public bool IsNullable { get; set; } = true;

	[TfDboModelProperty("is_unique")]
	public bool IsUnique { get; set; }

	[TfDboModelProperty("is_sortable")]
	public bool IsSortable { get; set; } = true;

	[TfDboModelProperty("is_searchable")]
	public bool IsSearchable { get; set; } = true;

	[TfDboModelProperty("include_in_table_search")]
	public bool IncludeInTableSearch { get; set; }

	//The formulate provided to PostgreSQL
	[TfDboModelProperty("expression")]
	public string? Expression { get; set; } = null;

	//Internal object that helps the user to generate and how the expression was generated 
	[TfDboModelProperty("expression_json")]
	public string? ExpressionJson { get; set; } = null;
	
	public bool IsReadOnly { get => !String.IsNullOrWhiteSpace(Expression); }

	public void FixPrefix(
		string prefix)
	{
		if (!String.IsNullOrWhiteSpace(DbName) && !DbName.StartsWith(prefix))
			DbName = prefix + DbName;
	}
	public void ApplyRuleSet(
		TfDataProviderColumnRuleSet ruleSet,
		string? defaultValue = null, 
		string? expression = null,
		string? expressionJson = null )
	{
		switch (ruleSet)
		{
			//1. IsNullable = true, IsUnique = false, AutoDefaultValue = false, DefaultValue = null
			//Nullable = 0,
			default:
				IsNullable = true;
				IsUnique = false;
				AutoDefaultValue = false;
				DefaultValue = null;
				break;
			//2. IsNullable = false, IsUnique = false, AutoDefaultValue = false, DefaultValue = REQUIRED
			case TfDataProviderColumnRuleSet.NullableWithDefault:
				IsNullable = false;
				IsUnique = false;
				AutoDefaultValue = false;
				DefaultValue = defaultValue ?? throw new Exception("Default Value is required");
				break;
			//3. IsNullable = false, IsUnique = false, AutoDefaultValue = true, DefaultValue = null
			case TfDataProviderColumnRuleSet.NullableWithAutoDefault:
				IsNullable = false;
				IsUnique = false;
				AutoDefaultValue = true;
				DefaultValue = null;
				break;
			//4. IsNullable = false, IsUnique = true, AutoDefaultValue = true, DefaultValue = null
			case TfDataProviderColumnRuleSet.Unique:
				IsNullable = false;
				IsUnique = true;
				AutoDefaultValue = true;
				DefaultValue = null;
				break;
		}
	}
	public TfDataProviderColumnRuleSet GetRuleSet()
	{
		//2. IsNullable = false, IsUnique = false, AutoDefaultValue = false, DefaultValue = REQUIRED
		if (!IsNullable && !IsUnique && !AutoDefaultValue && DefaultValue is not null)
			return TfDataProviderColumnRuleSet.NullableWithDefault;

		//3. IsNullable = false, IsUnique = false, AutoDefaultValue = true, DefaultValue = null
		if (!IsNullable && !IsUnique && AutoDefaultValue && DefaultValue is null)
			return TfDataProviderColumnRuleSet.NullableWithAutoDefault;

		//4. IsNullable = false, IsUnique = true, AutoDefaultValue = true, DefaultValue = null
		if (!IsNullable && IsUnique && AutoDefaultValue && DefaultValue is null)
			return TfDataProviderColumnRuleSet.Unique;

		//1.[Nullable] IsNullable = true, IsUnique = false, AutoDefaultValue = false, DefaultValue = null
		return TfDataProviderColumnRuleSet.Nullable;
	}

	public TfUpsertDataProviderColumn ToUpsert()
	{

		var model = new TfUpsertDataProviderColumn
		{
			Id = Id,
			DataProviderId = DataProviderId,
			SourceName = SourceName,
			SourceType = SourceType,
			CreatedOn = CreatedOn,
			DbName = DbName,
			DbType = DbType,
			IncludeInTableSearch = IncludeInTableSearch,
			DefaultValue = DefaultValue,
			RuleSet = this.GetRuleSet(),
			Expression = Expression,
			ExpressionJson = ExpressionJson,
		};
		return model;
	}
}


public enum TfDataProviderColumnRuleSet
{
	//1. IsNullable = true, IsUnique = false, AutoDefaultValue = false, DefaultValue = null
	[Description("not required - leave NULL when not provided")]
	Nullable = 0,
	//2. IsNullable = false, IsUnique = false, AutoDefaultValue = false, DefaultValue = REQUIRED
	[Description("required - fixed value when not provided")]
	NullableWithDefault = 1,
	//3. IsNullable = false, IsUnique = false, AutoDefaultValue = true, DefaultValue = null
	[Description("required - autogenerated value when not provided")]
	NullableWithAutoDefault = 2,
	//4. IsNullable = false, IsUnique = true, AutoDefaultValue = true, DefaultValue = null
	[Description("required and must have only unique values")]
	Unique = 3,
}

public enum TfDataProviderColumnDataInputType
{
	[Description("local - value")]
	LocalInput = 0,
	[Description("local - calculated with formula")]
	LocalCalculated = 1,
	[Description("imported - from the data source")]
	ImportFromSource = 2,	
}

public class TfUpsertDataProviderColumn
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public string? SourceName { get; set; }
	public string? SourceType { get; set; } = null;
	public DateTime CreatedOn { get; set; }
	public string? DbName { get; set; }
	public TfDatabaseColumnType DbType { get; set; } = TfDatabaseColumnType.Text;
	public string? DefaultValue { get; set; }
	public TfDataProviderColumnRuleSet RuleSet { get; set; } = TfDataProviderColumnRuleSet.Nullable;
	public bool IncludeInTableSearch { get; set; }
	public string? Expression { get; set; } = null;
	public string? ExpressionJson { get; set; } = null;

	public void FixPrefix(string prefix)
	{
		if (!String.IsNullOrWhiteSpace(DbName) && !DbName.StartsWith(prefix))
			DbName = prefix + DbName;
	}

	public TfDataProviderColumn ToModel()
	{
		var model = new TfDataProviderColumn
		{
			Id = Id,
			DataProviderId = DataProviderId,
			SourceName = SourceName,
			SourceType = SourceType,
			CreatedOn = CreatedOn,
			DbName = DbName,
			DbType = DbType,
			IncludeInTableSearch = IncludeInTableSearch,
			DefaultValue = null,//set below
			AutoDefaultValue = false,//set below
			IsNullable = false, //set below
			IsUnique = false,//set below
			IsSortable = true,//always
			IsSearchable = true,//always
			Expression = Expression,
			ExpressionJson = ExpressionJson
		};
		//Applies the ruleset props
		model.ApplyRuleSet(RuleSet, DefaultValue);

		return model;
	}
}