namespace WebVella.Tefter;

public enum TfFilterNumericComparisonMethod
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

public enum TfFilterTextComparisonMethod
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

public enum TfFilterBooleanComparisonMethod
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

public enum TfFilterGuidComparisonMethod
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

public enum TfFilterDateTimeComparisonMethod
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