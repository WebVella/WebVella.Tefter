namespace WebVella.Tefter.UseCases.Models;

[TucEnumMatch(typeof(TfSortDirection))]
public enum TucSortDirection
{
	[Description("Ascending")]
	ASC = 0,
	[Description("Descending")]
	DESC = 1
}
