namespace WebVella.Tefter;

public enum TfFilterNumericComparisonMethod
{
	Equal = 0,
	NotEqual = 1,
	Greater = 2,
	GreaterOrEqual = 4,
	Lower = 8,
	LowerOrEqual = 16,
	HasValue = 32,
	HasNoValue = 64
}

public enum TfFilterTextComparisonMethod
{
	Equal = 0,
	NotEqual = 1,
	StartsWith = 2,
	EndsWith = 4,
	Contains = 8,
	Fts = 16,
	HasValue = 32,
	HasNoValue = 64
}

public enum TfFilterBooleanComparisonMethod
{
	Equal = 0,
	NotEqual = 1,
	IsTrue = 2,
	IsFalse = 4,
	HasValue = 8,
	HasNoValue = 16
}

public enum TfFilterGuidComparisonMethod
{
	Equal = 0,
	NotEqual = 1,
	IsEmpty = 2,
	IsNotEmpty = 4,
	HasValue = 8,
	HasNoValue = 16
}

public enum TfFilterDateTimeComparisonMethod
{
	Equal = 0,
	NotEqual = 1,
	Greater = 2,
	GreaterOrEqual = 4,
	Lower = 8,
	LowerOrEqual = 16,
	HasValue = 32,
	HasNoValue = 64
}