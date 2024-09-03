namespace WebVella.Tefter.UseCases.Models;

[TucEnumMatch(typeof(TfFilterNumericComparisonMethod))]
public enum TucFilterNumericComparisonMethod
{
	[Description("equal")]
	Equal = 0,
	[Description("not equal")]
	NotEqual = 1,
	[Description("greater")]
	Greater = 2,
	[Description("greater or equal")]
	GreaterOrEqual = 4,
	[Description("lower")]
	Lower = 8,
	[Description("lower or equal")]
	LowerOrEqual = 16,
	[Description("has value")]
	HasValue = 32,
	[Description("has no value")]
	HasNoValue = 64
}

[TucEnumMatch(typeof(TfFilterTextComparisonMethod))]
public enum TucFilterTextComparisonMethod
{
	[Description("equal")]
	Equal = 0,
	[Description("not equal")]
	NotEqual = 1,
	[Description("starts with")]
	StartsWith = 2,
	[Description("ends with")]
	EndsWith = 4,
	[Description("contains")]
	Contains = 8,
	[Description("full text search")]
	Fts = 16,
	[Description("has value")]
	HasValue = 32,
	[Description("has no value")]
	HasNoValue = 64
}
[TucEnumMatch(typeof(TfFilterBooleanComparisonMethod))]
public enum TucFilterBooleanComparisonMethod
{
	[Description("equal")]
	Equal = 0,
	[Description("not equal")]
	NotEqual = 1,
	[Description("is true")]
	IsTrue = 2,
	[Description("is false")]
	IsFalse = 4,
	[Description("has value")]
	HasValue = 8,
	[Description("has no value")]
	HasNoValue = 16
}

[TucEnumMatch(typeof(TfFilterGuidComparisonMethod))]
public enum TucFilterGuidComparisonMethod
{
	[Description("equal")]
	Equal = 0,
	[Description("not equal")]
	NotEqual = 1,
	[Description("is empty")]
	IsEmpty = 2,
	[Description("is not empty")]
	IsNotEmpty = 4,
	[Description("has value")]
	HasValue = 8,
	[Description("has no value")]
	HasNoValue = 16
}
[TucEnumMatch(typeof(TfFilterDateTimeComparisonMethod))]
public enum TucFilterDateTimeComparisonMethod
{
	[Description("equal")]
	Equal = 0,
	[Description("not equal")]
	NotEqual = 1,
	[Description("greater")]
	Greater = 2,
	[Description("greater or equal")]
	GreaterOrEqual = 4,
	[Description("lower")]
	Lower = 8,
	[Description("lower or equal")]
	LowerOrEqual = 16,
	[Description("has value")]
	HasValue = 32,
	[Description("has no value")]
	HasNoValue = 64
}