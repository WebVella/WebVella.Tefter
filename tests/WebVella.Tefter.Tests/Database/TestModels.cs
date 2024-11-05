namespace WebVella.Tefter.Tests.Database;

[DboCacheModel]
[TfDboModel("test_table")]
public class TestModel
{
	[TfDboModelProperty("id")] public Guid Id { get; set; }
	[TfDboModelProperty("checkbox")] public bool Checkbox { get; set; }
	[TfDboModelProperty("checkbox_null")] public bool? CheckboxNull { get; set; }
	[TfDboModelProperty("date")] public DateTime Date { get; set; }
	[TfDboModelProperty("date_null")] public DateTime? DateNull { get; set; }
	[TfDboModelProperty("datetime")] public DateTime DateTime { get; set; }
	[TfDboModelProperty("datetime_null")] public DateTime? DateTimeNull { get; set; }
	[TfDboModelProperty("number")] public decimal Number { get; set; }
	[TfDboModelProperty("number_null")] public decimal? NumberNull { get; set; }
	[TfDboModelProperty("guid")] public Guid Guid { get; set; }
	[TfDboModelProperty("guid_null")] public Guid? GuidNull { get; set; }
	[TfDboModelProperty("text")] public string Text { get; set; }
	[TfDboModelProperty("json_obj")] public string JsonObj { get; set; }
	[TfDboModelProperty("json_list")] public string JsonList { get; set; }
}


[DboCacheModel]
[TfDboModel("test_table")]
public class NumberTestModel
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("number")]
	[TfDboTypeConverter(typeof(TfIntegerPropertyConverter))]
	public int Number { get; set; }

	[TfDboModelProperty("number_null")]
	[TfDboTypeConverter(typeof(TfIntegerPropertyConverter))]
	public int? NumberNull { get; set; }
}



[DboCacheModel]
[TfDboModel("test_table")]
public class JsonTestModel
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("json_obj")]
	[TfDboTypeConverter(typeof(TfJsonPropertyConverter<JsonObject>))]
	public JsonObject JsonObj { get; set; }

	[TfDboModelProperty("json_list")]
	[TfDboTypeConverter(typeof(TfJsonPropertyConverter<List<JsonObject>>))]
	public List<JsonObject> JsonList { get; set; }
}

public record JsonObject
{
	public string Name { get; set; }
	public DateTime Date { get; set; }
}


[DboCacheModel]
[TfDboModel("test_table")]
public class LongNumberTestModel
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("number")]
	[TfDboTypeConverter(typeof(TfLongPropertyConverter))]
	public long Number { get; set; }

	[TfDboModelProperty("number_null")]
	[TfDboTypeConverter(typeof(TfLongPropertyConverter))]
	public long? NumberNull { get; set; }
}



[DboCacheModel]
[TfDboModel("test_table")]
public class EnumTestModel
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("short")]
	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<SampleEnumType>))]
	public SampleEnumType Sample { get; set; }

	[TfDboModelProperty("short_null")]
	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<SampleEnumType>))]
	public SampleEnumType? SampleNull { get; set; }
}

public enum SampleEnumType
{
	EnumValue1 = 1,
	EnumValue2 = 2,
	EnumValue3 = 3
}


[DboCacheModel]
[TfDboModel("test_table")]
public class DateTimeTestModel
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("datetime")]
	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	public DateTime Date { get; set; }

	[TfDboModelProperty("datetime_null")]
	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	public DateTime? DateNull { get; set; }
}

[DboCacheModel]
[TfDboModel("test_table")]
public class DateTestModel
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("date")]
	[TfDboTypeConverter(typeof(TfDatePropertyConverter))]
	public DateTime Date { get; set; }

	[TfDboModelProperty("date_null")]
	[TfDboTypeConverter(typeof(TfDatePropertyConverter))]
	public DateTime? DateNull { get; set; }
}