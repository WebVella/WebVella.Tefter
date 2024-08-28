namespace WebVella.Tefter.UseCases.Models;

[TucEnumMatch(typeof(TfDataProviderColumnSearchType))]
public enum TucDataProviderColumnSearchType
{
	Equals,
	Comparison,
	Contains
}
