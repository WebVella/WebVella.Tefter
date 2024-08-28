namespace WebVella.Tefter.UseCases.Models;

[TucEnumMatch(typeof(TfFilterNumericComparisonMethod))]
public enum TucFilterNumericComparisonMethod
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

[TucEnumMatch(typeof(TfFilterTextComparisonMethod))]
public enum TucFilterTextComparisonMethod
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
[TucEnumMatch(typeof(TfFilterBooleanComparisonMethod))]
public enum TucFilterBooleanComparisonMethod
{
	Equal = 0,
	NotEqual = 1,
	IsTrue = 2,
	IsFalse = 4,
	HasValue = 8,
	HasNoValue = 16
}

[TucEnumMatch(typeof(TfFilterGuidComparisonMethod))]
public enum TucFilterGuidComparisonMethod
{
	Equal = 0,
	NotEqual = 1,
	IsEmpty = 2,
	IsNotEmpty = 4,
	HasValue = 8,
	HasNoValue = 16
}
[TucEnumMatch(typeof(TfFilterDateTimeComparisonMethod))]
public enum TucFilterDateTimeComparisonMethod
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