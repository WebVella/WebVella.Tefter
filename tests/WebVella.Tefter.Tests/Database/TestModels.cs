namespace WebVella.Tefter.Tests.Database;

[DboCacheModel]
[DboModel("test_table")]
public class TestModel
{
	[DboModelProperty("id")] public Guid Id { get; set; }
	[DboModelProperty("checkbox")] public bool Checkbox { get; set; }
	[DboModelProperty("checkbox_null")] public bool? CheckboxNull { get; set; }
	[DboModelProperty("date")] public DateTime Date { get; set; }
	[DboModelProperty("date_null")] public DateTime? DateNull { get; set; }
	[DboModelProperty("datetime")] public DateTime DateTime { get; set; }
	[DboModelProperty("datetime_null")] public DateTime? DateTimeNull { get; set; }
	[DboModelProperty("number")] public decimal Number { get; set; }
	[DboModelProperty("number_null")] public decimal? NumberNull { get; set; }
	[DboModelProperty("guid")] public Guid Guid { get; set; }
	[DboModelProperty("guid_null")] public Guid? GuidNull { get; set; }
	[DboModelProperty("text")] public string Text { get; set; }
	[DboModelProperty("json_obj")] public string JsonObj { get; set; }
	[DboModelProperty("json_list")] public string JsonList { get; set; }
}


[DboCacheModel]
[DboModel("test_table")]
public class NumberTestModel
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("number")]
	[DboTypeConverter(typeof(IntegerPropertyConverter))]
	public int Number { get; set; }

	[DboModelProperty("number_null")]
	[DboTypeConverter(typeof(IntegerPropertyConverter))]
	public int? NumberNull { get; set; }
}



[DboCacheModel]
[DboModel("test_table")]
public class JsonTestModel
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("json_obj")]
	[DboTypeConverter(typeof(JsonPropertyConverter<JsonObject>))]
	public JsonObject JsonObj { get; set; }

	[DboModelProperty("json_list")]
	[DboTypeConverter(typeof(JsonPropertyConverter<List<JsonObject>>))]
	public List<JsonObject> JsonList { get; set; }
}

public record JsonObject
{
	public string Name { get; set; }
	public DateTime Date { get; set; }
}


[DboCacheModel]
[DboModel("test_table")]
public class LongNumberTestModel
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("number")]
	[DboTypeConverter(typeof(LongPropertyConverter))]
	public long Number { get; set; }

	[DboModelProperty("number_null")]
	[DboTypeConverter(typeof(LongPropertyConverter))]
	public long? NumberNull { get; set; }
}



[DboCacheModel]
[DboModel("test_table")]
public class EnumTestModel
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("number")]
	[DboTypeConverter(typeof(EnumPropertyConverter<SampleEnumType>))]
	public SampleEnumType Sample { get; set; }

	[DboModelProperty("number_null")]
	[DboTypeConverter(typeof(EnumPropertyConverter<SampleEnumType>))]
	public SampleEnumType? SampleNull { get; set; }
}

public enum SampleEnumType
{
	EnumValue1 = 1,
	EnumValue2 = 2,
	EnumValue3 = 3
}


[DboCacheModel]
[DboModel("test_table")]
public class DateTimeTestModel
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("date")]
	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	public DateTime Date { get; set; }

	[DboModelProperty("date_null")]
	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	public DateTime? DateNull { get; set; }
}